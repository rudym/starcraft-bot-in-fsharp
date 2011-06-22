module StarCraftBot9K.StarCraft.Communication

open System
open System.Text
open System.Net
open System.Net.Sockets
open System.Net.NetworkInformation

#if CLR40
open System.Collections.Concurrent
#else
type ConcurrentQueue<'a> = System.Collections.Generic.Queue<'a>

type System.Collections.Generic.Queue<'a> with
    member this.TryDequeue(x : byref<'a>) =
        x <- this.Dequeue()
        true

#endif

open StarCraftBot9K.AI.AIBase
open StarCraftBot9K.StarCraft.BasicOM

[<AllowNullLiteralAttribute>]
type ISocketCommunicator =
    abstract ReadLine  : unit -> string
    abstract WriteLine : string -> unit

let getCommunicator (socket : Socket) = 
    let buffer = Array.create (1024 * 1024) 0uy
    
    let translateData bytesRead = 
        Array.init bytesRead (fun i -> buffer.[i]) 
        |> Array.map (fun byte -> Convert.ToChar(byte)) 
        |> (fun chars -> new String(chars))
    
    let encodeData (msg : string) =
        msg.ToCharArray()
        |> Array.map (fun c -> Convert.ToByte(c))
    
    { 
        new ISocketCommunicator with
            member this.ReadLine() =
                let bytesRead = socket.Receive(buffer)
                translateData bytesRead
            member this.WriteLine msg =
                let msg' = encodeData msg
                socket.Send(msg') |> ignore
    }

/// Default port number when listening for the StarCraft instance
let DefaultPortNumber = 12620

type CommState =
    | NotConnected        = 0
    | WaitingForStarCraft = 1
    | ListeningForUpdates = 2
    | ShuttingDown        = 3

type HandshakeCompleteDelegate   = delegate of sender:obj * args:EventArgs -> unit
type PlayerStateRecievedDelegate = delegate of sender:obj * state:PlayerState -> unit

/// The communication layer between ProxyBot and StarCraft. Opens up
/// a port for communication.
type StarCraftConnector(port) =

    // Socket communication
    let getHostIP() =
        let hostName = System.Net.Dns.GetHostName()
        // Get the IPV4 address...
        let ipv4 = 
            System.Net.Dns.GetHostAddresses(hostName)
            |> Array.tryFind (fun ip -> ip.AddressFamily = AddressFamily.InterNetwork)
        match ipv4 with
        | Some(ip) -> ip
        | None     -> failwith "Unable to get IPV4 IP address"

    let mutable m_flagAllowUserInput = false
    let mutable m_flagGivePerfectInformation = false

    let m_tcpServer = new TcpListener(getHostIP(), port)
    
    let mutable m_socket     : Socket = null
    let mutable m_socketComm : ISocketCommunicator = null

    let mutable m_state = CommState.NotConnected

    // Game information
    let mutable m_gameMetadata : GameMetaData option = None
    let mutable m_commandQueue = new ConcurrentQueue<SCCommand>()

    // Eventing
    let m_handshakeCompleteEvent  = new Event<HandshakeCompleteDelegate,   EventArgs>()
    let m_playerStateUpdatedEvent = new Event<PlayerStateRecievedDelegate, PlayerState>()

    // ------------------------------------------------------------------------

    interface IDisposable with
        override this.Dispose() = this.ShutDown()

    // ------------------------------------------------------------------------

    /// Connect to an instance of StarCraft using the default port
    new() = new StarCraftConnector(DefaultPortNumber)

    member this.FlagAllowUserInput 
        with get () = m_flagAllowUserInput
        and  set x  = match m_state with
                      | CommState.NotConnected
                      | CommState.WaitingForStarCraft 
                          -> m_flagAllowUserInput <- x
                      | _ -> raise <| new InvalidOperationException("Cannot set flags after initialization.")

    member this.FlagGivePerfectInformation
        with get () = m_flagGivePerfectInformation
        and  set x  = match m_state with
                      | CommState.NotConnected
                      | CommState.WaitingForStarCraft 
                          -> m_flagGivePerfectInformation <- x
                      | _ -> raise <| new InvalidOperationException("Cannot set flags after initialization.")

    /// Game metadata
    member this.GameMetadata =
        match m_gameMetadata with
        | Some(gmd) -> gmd
        | None -> raise <| new InvalidOperationException("Game metadata not loaded yet!")

    /// Current state of communication
    member this.State with get () = m_state
                      and  set desiredState  = 
                        // Make sure all transitions are legit
                        match m_state, desiredState with
                        | CommState.NotConnected,        CommState.WaitingForStarCraft 
                        | CommState.WaitingForStarCraft, CommState.ListeningForUpdates
                        | CommState.ListeningForUpdates, CommState.ShuttingDown
                        | CommState.ShuttingDown,        CommState.NotConnected
                            -> m_state <- desiredState
                        | _ -> raise <| new InvalidOperationException("Invalid state transition")
                    

    /// Shuts down the underlying socket
    member this.ShutDown() =
        // Indicate to the socket reading thread to shut down
        this.State <- CommState.ShuttingDown
        Threading.Thread.Sleep(1000)
        
        // Close things out
        if m_socket <> null then
            m_socket.Shutdown(SocketShutdown.Both)
            m_socket <- null
        m_tcpServer.Stop()        
        
        // If you wanted to open the socket again, go ahead
        this.State <- CommState.NotConnected

    member this.QueueCommand(cmd : SCCommand) =
        m_commandQueue.Enqueue(cmd)
        

    /// Begin listening for an instance of StarCraft asynchronously. Will raise the
    /// HandshakeComplete event when finished.    
    member this.BeginListening() =

        if m_state <> CommState.NotConnected then
            raise <| new InvalidOperationException("Already listening to StarCraft instance.")
            
        m_tcpServer.Start()
            
        async {
            // Asssumes m_tcpServer has already been started in the constructor

            // Wait for StarCraft to connect
            this.State <- CommState.WaitingForStarCraft
            m_socket   <- m_tcpServer.AcceptSocket(
                                Blocking = false, 
                                SendTimeout = 3 * 1000, 
                                ReceiveBufferSize = 1024 * 1024)

            // Now the instance is created, wrap all socket IO
            m_socketComm <- getCommunicator m_socket

            // Read the welcome message
            // Should look a lot like:
            // "ProxyBotACK;playerID;playerRace;enemyID;enemyRace"
            let welcomeMessage = m_socketComm.ReadLine()

            // Send bot options that StarCraft is waiting for
            let botOptions =
                let flags = [| '0'; '0' |]
                if m_flagAllowUserInput         then flags.[0] <- '1'
                if m_flagGivePerfectInformation then flags.[1] <- '1'
                new String(flags)
            m_socketComm.WriteLine(botOptions)
            
            // The first update blends in with the first status update. Just read until
            // there is no more data available.
            do! Async.Sleep(2500)
            let metaData, firstUpdate = 
                let dataRead = new StringBuilder(10 * 1024)
                while m_socket.Available <> 0 do
                    dataRead.Append(m_socketComm.ReadLine()) |> ignore
                    Threading.Thread.Sleep(100)
                let data = dataRead.ToString()
                let statusStart = data.IndexOf("status")
                (data.Substring(0, statusStart), data.Substring(statusStart))
        
            let metadata = GameMetaData.Parse(metaData)
            m_gameMetadata <- Some(metadata)
            
            // Set the global instance of game metadata
            g_GameMetadata <- metadata
            
            let initialPlayerState = PlayerState.Parse(firstUpdate)
        
            // Send empty command for the first update
            m_socketComm.WriteLine("Commands;")

            // Everything should be complete. From now on StarCraft will be sending
            // status updates every few ticks.
            
            // Fire the complete event
            this.State <- CommState.ListeningForUpdates
            m_handshakeCompleteEvent.Trigger(this, new EventArgs())
            
            // Now just read all the status updates from StarCraft (one per line)
            // allow listeners to hook up to these events
            while this.State <> CommState.ShuttingDown do
                while m_socket.Available = 0 do
                    do! Async.Sleep(1)
                    
                let rawPlayerState = m_socketComm.ReadLine()
                if m_socket.Available <> 0 then failwith "Didn't read all data?"
                
                m_playerStateUpdatedEvent.Trigger(this, PlayerState.Parse(rawPlayerState))

                // Get the command queue ready for transmission
                let commandBuffer = new StringBuilder("Commands")
                while m_commandQueue.Count > 0 do
                    let cmd = ref (HoldPosition(0))
                    while not <| m_commandQueue.TryDequeue(cmd) do
                        // Couldn't dequeue due to race condition. Wait and retry
                        do! Async.Sleep(1)

                    commandBuffer.Append(cmd.Value.ToString()) |> ignore
                m_socketComm.WriteLine(commandBuffer.ToString())
            
            // Finished
            this.ShutDown()
            
            m_gameMetadata <- None
            this.State <- CommState.NotConnected
        } |> Async.Start
        
        // We've started the task asynchronously, it'll finish later
        ()

    // ------------------------------------------------------------------------
    
    // Event fired when handshake is complete
    [<CLIEvent>]
    member this.HandshakeComplete = m_handshakeCompleteEvent.Publish
    
    [<CLIEvent>]
    member this.GameStateUpdated = m_playerStateUpdatedEvent.Publish

/// Regular commands and game updates for AI modules. Properties will be kept up to date
type GameMediator(socketComm : StarCraftConnector) =

    let mutable m_currentGameState = 
        { 
            Minerals    = -1
            Gas         = -1
            SupplyUsed  = -1
            SupplyTotal = -1
            CanProduce  = [| |]
            CanTech     = [| |]
            CanUpgrade  = [| |]
            Units       = [| |]
        }

    // Keep the current game state up to date with updates
    do socketComm.add_GameStateUpdated(fun sender updatedState -> m_currentGameState <- updatedState)

    /// Returns the current game state. THIS MAY BE UPDATED WITHOUT YOUR KNOWING! It gets updated on each tick.
    member this.CurrentGameState = m_currentGameState
    
    member this.GameMetadata = socketComm.GameMetadata

    member this.SendCommand(cmd : SCCommand) =
        socketComm.QueueCommand(cmd)