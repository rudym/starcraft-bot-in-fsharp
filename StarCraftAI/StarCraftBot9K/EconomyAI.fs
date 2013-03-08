module StarCraftBot9K.AI.EconomyAI

open StarCraftBot9K.StarCraft.Constants
open StarCraftBot9K.StarCraft.BasicOM
open StarCraftBot9K.StarCraft.Communication
open StarCraftBot9K.AI.AIBase
open StarCraftBot9K.AI.AIStructs

/// AI module for managaing your economy. Building drones, vespen geysers, etc.
let private getEconomyAI (mediator : GameMediator) =
    async {

        let currentStateObj = ref mediator.CurrentGameState
        
        while true do

            let currentState = mediator.CurrentGameState
                    
            // Build workers with all available cash, but without queueing
            if currentState.SupplyUsed < currentState.SupplyTotal &&
               currentState.Minerals >= 50 &&
               currentState.CanProduce.[ int (getWorkerType(g_GameMetadata.PlayerRace)) ] then
               
                // BUG: Builds a worker at the first command center we find, this might not be the best one.
                // Ideally we'd have some higher level notion of a 'base' with a 'status' such as 'active' or 'out of resources'.
                let cmdCenter = 
                    currentState.Units 
                    |> Seq.filter (fun unit -> unit.Player = g_GameMetadata.PlayerID)
                    |> Seq.tryFind (fun unit -> isCommandCenter (enum<UnitID> unit.TypeID))
                    //|> Seq.tryFind (fun unit -> isBuilding (enum<UnitID> unit.ID))
                    
                if Option.isSome cmdCenter then
                    let cmdCenterID, isTraining = cmdCenter |> Option.get |> (fun unit -> unit.ID, unit.isTraining)                    
                    if isTraining = 0 then
                        let cmd = TrainUnit(cmdCenterID, int (getWorkerType(g_GameMetadata.PlayerRace)))
                        mediator.SendCommand(cmd)

            // Send idle workers to closest mineral patch
            for scunit in currentState.Units do
                // If you own the unit, it's a worker, and it's sitting there then
                // send it to the closest mineral patch.
                if scunit.Player = mediator.GameMetadata.PlayerID &&
                   isWorker scunit &&
                   scunit.OrderID = int (UnitOrder.PlayerGuard) then
                       
                    let orderID = scunit.OrderID

                    let scunitLoc = { X = scunit.XPos; Y = scunit.YPos }

                    let mineralPatches = 
                        currentState.Units
                        |> Seq.filter(fun unit -> unit.TypeID = int UnitID.MineralField)
                        |> Seq.map(fun minPatch -> let patchLoc = { X = minPatch.XPos; Y = minPatch.YPos }
                                                   scunitLoc.FlyingDistanceTo(patchLoc), minPatch)
                        |> Seq.sortBy(fun (dist, minPatch) -> dist)
                        
                    if mineralPatches <> Seq.empty then
                        let targetMinPatch = mineralPatches |> Seq.head |> snd
                        let cmd = RightClickUnit(scunit.ID, targetMinPatch.ID)
                        mediator.SendCommand(cmd)

            // Sleep and check again later. If this agent gets canceled it will be right here.
            do! Async.Sleep(100)
        }

open System.Threading

/// Keep track of our economy AI by its cancellation token source
let mutable private  m_cts = null

let startEconomyAI mediator = 
    match m_cts with
    | null -> m_cts <- new CancellationTokenSource()
              Async.Start(getEconomyAI mediator, m_cts.Token)
    | _ -> failwith "Error: AI already started!"

let stopEconomyAI() =
    match m_cts with
    | null -> failwith "Error: AI already started!"
    | _ -> m_cts.Cancel()
           m_cts <- null // Zero it out so we can potentially start again
    