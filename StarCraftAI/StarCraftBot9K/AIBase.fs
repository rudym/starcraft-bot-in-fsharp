module StarCraftBot9K.AI.AIBase

open StarCraftBot9K.StarCraft.BasicOM

type SCCommandID =
    | RightClickPos  = 3
    | RightClickUnit = 4
    | TrainUnit      = 5
    | HoldPosition   = 11

type SCCommand =
    /// Right click on a location. (Only means move - to mine, right click a unit.)
    | RightClickPos  of int * Location
    /// Right click on a unit. Typically means attack or mine.
    | RightClickUnit of int * int
    /// Command a building unit to produce another unit
    | TrainUnit      of int * int
    /// Hold position. Stop where you are, attack anythying nearby.
    | HoldPosition   of int
    ///Command a free worker to build a new builing
    ///| Build          of int * Location

    /// Generates the command in a format to be transfered to bwapi. THESE MUST BE IN SYNC.
    override this.ToString() =
        let printCmd3 (cmd : SCCommandID) unitID arg0 arg1 arg2 =
            sprintf ":%d;%d;%d;%d;%d" (int cmd) unitID arg0 arg1 arg2
        let printCmd2 cmd u a0 a1 = printCmd3 cmd u a0 a1 0
        let printCmd1 cmd u a0    = printCmd3 cmd u a0 0  0
        let printCmd0 cmd u       = printCmd3 cmd u 0  0  0

        match this with
        | RightClickPos(u, loc)  -> printCmd2 SCCommandID.RightClickPos u loc.X loc.Y
        | RightClickUnit(u, tgt) -> printCmd1 SCCommandID.RightClickUnit u tgt
        | TrainUnit(u, newUID)   -> printCmd1 SCCommandID.TrainUnit u newUID
        | HoldPosition(u)        -> printCmd0 SCCommandID.HoldPosition u