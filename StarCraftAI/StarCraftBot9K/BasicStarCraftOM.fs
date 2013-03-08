module StarCraftBot9K.StarCraft.BasicOM

open System
open StarCraftBot9K.StarCraft.Constants

type BasicUnitInfo =
    {
        ID     : int
        Player : int    // Owner
        TypeID : int
        XPos   : int
        YPos   : int
        HitPoints : int
        Shields   : int
        Energy    : int
        OrderTimer : int
        OrderID    : int
        isTraining : int
        Resources : int
    }
    static member Parse(str : string) =
        let data = str.Split([| ',' |])
        {
            ID     = Int32.Parse(data.[0])
            Player = Int32.Parse(data.[1])
            TypeID = Int32.Parse(data.[2])
            XPos   = Int32.Parse(data.[3])
            YPos   = Int32.Parse(data.[4])
            HitPoints = Int32.Parse(data.[5])
            Shields   = Int32.Parse(data.[6])
            Energy    = Int32.Parse(data.[7])
            OrderTimer = Int32.Parse(data.[8])
            OrderID    = Int32.Parse(data.[9])
            isTraining  = Int32.Parse(data.[10])
            Resources  = Int32.Parse(data.[11])
        }
    override this.ToString() =
        sprintf 
            "Unit {ID = %d, TypeID = %d, HP = %d, OrderID = %d}"
            this.ID
            this.TypeID
            this.HitPoints
            this.OrderID

let isWorker bui =
    match enum<UnitID> bui.TypeID with
    | UnitID.TerranSCV 
    | UnitID.ZergDrone
    | UnitID.ProtosProbe -> true
    | _ -> false
    
let getWorkerType raceId =
    match enum<Race> raceId with
    | Race.Terran -> UnitID.TerranSCV
    | Race.Zerg   -> UnitID.ZergDrone
    | Race.Protos -> UnitID.ProtosProbe
    | _ -> failwithf "Unknown race %A" (enum<Race> raceId)
    
let isCommandCenter typeID =
    match typeID with
    | UnitID.TerranCommandCenter
    | UnitID.ZergHatchery
    | UnitID.ProtosNexus 
        -> true
    | _ -> false

let isBuilding bui =
    match enum<UnitID> bui.TypeID with
    | UnitID.TerranSCV 
    | UnitID.ZergDrone
    | UnitID.ProtosProbe -> true
    | _ -> false
    
let getUnitName bui =   
    let unitName = enum<UnitID> bui.TypeID
    let name = unitName.ToString()
    // Print unknown if it isn't in our list
    let parsedResult = ref UnitID.Unknown
#if CLR40
    if not <| Enum.TryParse(name, parsedResult) then
        "Unknown"
    else
        name
#else
    name
#endif

type PlayerState =
    {
        Minerals    : int
        Gas         : int
        SupplyUsed  : int   // Need more Overlords!
        SupplyTotal : int   
        CanProduce : bool[] // 230
        CanUpgrade : bool[] // 63
        CanTech    : bool[] // 47
        Units : BasicUnitInfo[]
    }
    static member Parse(status : string) =
        // Parse a bit vector encoded as a string
        let parseBits (str : string) =
            let result = Array.zeroCreate str.Length
            Seq.iteri
                (fun idx x -> match x with
                              | '1' -> result.[idx] <- true
                              | '0' -> result.[idx] <- false
                              | _ -> failwithf "Invalid bit %c" x)
                str
            result

        let parseUnits (str : string) =
            str.Split([| '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map BasicUnitInfo.Parse

        let parts = status.Split([| ';' |])
        if parts.Length = 0 || parts.[0] <> "status" then
            if status.Length > 0 then
                raise <| new ArgumentException(sprintf "Invalid status update '%s'..." <| status.Substring(0, 50))
            else
                raise <| new ArgumentException(sprintf "Invalid status update '%s'" status)
        else
            let gameState =
                { 
                    Minerals    = Int32.Parse(parts.[1])
                    Gas         = Int32.Parse(parts.[2])
                    SupplyUsed  = Int32.Parse(parts.[3])
                    SupplyTotal = Int32.Parse(parts.[4])
                    CanProduce  = parseBits parts.[5]
                    CanTech     = parseBits parts.[6]
                    CanUpgrade  = parseBits parts.[7]
                    Units       = parseUnits parts.[8]
                }            
            gameState
    
type UnitType =
    {
        ID      : int
        Race    : string
        Name    : string
        MineralCost : int
        GasCost : int
        HitPoints : int
        Shields : int
        Energy  : int
        BuildTime : int
        CanAttack : string
        CanMove : string
        Width   : int
        Height  : int
        SupplyRequired  : string    // TODO: Clean this up. Store in an int bit field?
        SupplyProvided  : string
        SightRange      : string
        GroundMaxRange  : string
        GroundMinRange  : string
        GroundDamage    : string
        AirRange        : string
        AirDamage       : string
        IsBuilding      : string
        IsFlyer         : string
        IsSpellCaster   : string
        Worker          : string
        CanBuildAddon   : string
        WhatBuilds      : string
    }
    static member Parse(str : string) =
        let parts = str.Split([| ',' |])
        {
            ID          = Int32.Parse(parts.[0])
            Race        = parts.[1]
            Name        = parts.[2]
            MineralCost = Int32.Parse(parts.[3])
            GasCost     = Int32.Parse(parts.[4])
            HitPoints   = Int32.Parse(parts.[5])
            Shields     = Int32.Parse(parts.[6])
            Energy      = Int32.Parse(parts.[7])
            BuildTime   = Int32.Parse(parts.[8])
            CanAttack   = parts.[9]
            CanMove     = parts.[10]
            Width       = Int32.Parse(parts.[11])
            Height      = Int32.Parse(parts.[12])
            SupplyRequired  = parts.[13]
            SupplyProvided  = parts.[14]
            SightRange      = parts.[15]
            GroundMaxRange  = parts.[16]
            GroundMinRange  = parts.[17]
            GroundDamage    = parts.[18]
            AirRange        = parts.[19]
            AirDamage       = parts.[20]
            IsBuilding      = parts.[21]//Boolean.Parse(parts.[21])
            IsFlyer         = parts.[22]
            IsSpellCaster   = parts.[23]
            Worker          = parts.[24]
            CanBuildAddon   = parts.[25]
            WhatBuilds      = parts.[26]
        }
        
type Location = 
    { 
        X : int
        Y : int 
    }
    static member Parse(str : string) =
        let parts = str.Split([| ',' |])
        { 
            X = Int32.Parse(parts.[0])
            Y = Int32.Parse(parts.[1])
        }
    /// Returns the distance between two points not taking terrain into account
    member this.FlyingDistanceTo(target) =
        let sqr x = x * x
        sqrt <| float (sqr (this.X - target.X) + sqr (this.Y - target.Y))
        
type GameMap =
    { 
        Name   : string
        Width  : int
        Height : int

        HeightAt  : int[,]
        Buildable : bool[,]
        Walkable  : bool[,]
    }
    
type TechType =
    {
        ID   : int
        Name : string
        WhatResearchers : int
        MineralPrice    : int
        GasPrice        : int
    }
    static member Parse (str : string) =
        let parts = str.Split([| ',' |])
        {
            ID   = Int32.Parse(parts.[0])
            Name = parts.[1]
            WhatResearchers = Int32.Parse(parts.[2])
            MineralPrice    = Int32.Parse(parts.[3])
            GasPrice        = Int32.Parse(parts.[4])
        }
        
type UpgradeType =
    {
        ID   : int
        Name : string
        WhatUpgrades : int
        MaxRanks     : int
        MineralBasePrice   : int
        MineralPriceFactor : int
        GasBasePrice   : int
        GasPriceFactor : int
    }
    static member Parse (str : string) =
        let parts = str.Split([| ',' |])
        {
            ID   = Int32.Parse(parts.[0])
            Name = parts.[1]
            WhatUpgrades       = Int32.Parse(parts.[2])
            MaxRanks           = Int32.Parse(parts.[3])
            MineralBasePrice   = Int32.Parse(parts.[4])
            MineralPriceFactor = Int32.Parse(parts.[5])
            GasBasePrice       = Int32.Parse(parts.[6])
            GasPriceFactor     = Int32.Parse(parts.[7])
        }
     
type GameMetaData =
    {
        PlayerID       : int
        PlayerRace     : int
        EnemyID        : int
        EnemyRace      : int
        UnitTypes      : UnitType[]
        StartLocations : Location[]
        Map            : GameMap
        TechTypes      : TechType[]
        UpgradeTypes   : UpgradeType[]
    }
    static member Parse(metadata : string) =   
        let parts = metadata.Split([| ';' |])
        if parts.[0] <> "PlayerInfo" then raise <| new ArgumentException()
        let playerID, playerRace, enemyID, enemyRace =
            let playerInfo = parts.[1].Split([| ',' |])
            (Int32.Parse(playerInfo.[0]), Int32.Parse(playerInfo.[1]), Int32.Parse(playerInfo.[2]), Int32.Parse(playerInfo.[3]))

        if parts.[2] <> "UnitTypes" then raise <| new ArgumentException()
        let unitTypes = 
            parts.[3].Split([| '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map UnitType.Parse

        if parts.[4] <> "StartLocations" then raise <| new ArgumentException()
        let startLocations =
            parts.[5].Split([| '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map Location.Parse
            
        if parts.[6] <> "MapHeader" then raise <| new ArgumentException()
        if parts.[8] <> "MapData" then raise <| new ArgumentException()
        let headerParts = parts.[7].Split([| ',' |])
        let width, height = Int32.Parse(headerParts.[1]), Int32.Parse(headerParts.[2])
        let map =
            {
                Name   = headerParts.[0]
                Width  = width
                Height = height
                HeightAt  = Array2D.zeroCreate width height
                Buildable = Array2D.zeroCreate width height
                Walkable  = Array2D.zeroCreate width height
            }
        // Now flesh out cells
        let parseBool = function '0' -> false | '1' -> true | _ -> failwith "arg exn"
        let mapData = parts.[9]
        for y in 0 .. height - 1 do
            for x in 0 .. width - 1 do
                let idx = (y * width + x) * 3
                map.HeightAt.[x,y]  <- int mapData.[idx + 0] - int '0'
                map.Buildable.[x,y] <- parseBool mapData.[idx + 1]
                map.Walkable.[x,y]  <- parseBool mapData.[idx + 2]
              
        if parts.[10] <> "TechTypes" then raise <| new ArgumentException()  
        let techTypes =
            parts.[11].Split([| '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map TechType.Parse
            
        if parts.[12] <> "UpgradeTypes" then raise <| new ArgumentException()
        let upgradeTypes =
            parts.[13].Split([| '|' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.map UpgradeType.Parse
            
        {
            PlayerID       = playerID
            PlayerRace     = playerRace
            EnemyID        = enemyID
            EnemyRace      = enemyRace
            UnitTypes      = unitTypes
            StartLocations = startLocations
            Map            = map
            TechTypes      = techTypes
            UpgradeTypes   = upgradeTypes
        }
        
/// Global copy of the current instance of StarCraft's metadata.
/// These values are read on startup, but for all practical purposes
/// the data is read only. (Unit costs and names won't change until SC is patched.)
let mutable g_GameMetadata =
    {
        PlayerID       = -1
        PlayerRace     = -1
        EnemyID        = -1
        EnemyRace      = -1
        UnitTypes      = [| |]
        StartLocations = [| |]
        Map            =
            { 
                Name   = "(No connection established yet)"
                Width  = -1
                Height = -1

                HeightAt  = null
                Buildable = null
                Walkable  = null
            }
        TechTypes      = [| |]
        UpgradeTypes   = [| |]
    }
