module StarCraftBot9K.StarCraft.Constants

(* NOTE: These values must correspond to constants defined in BWAPI *)

let NumUnitTypes    = 230
let NumTechTypes    = 47
let NumUpgradeTypes = 63

/// Race values passed in from SC
type Race =
    | Zerg    = 0
    | Terran  = 1
    | Protos  = 2
    | Random  = 4
    | None    = 5
    | Unknown = 6
    
type UnitID =
    // ---- Units ----
    // Terran Unit IDs
    | TerranSCV = 7

    // Zerg Unit IDs
    | ZergDrone = 41
    
    // Protos Unit IDs
    | ProtosProbe = 64
    
    // ---- Building ----
    // Terran Building IDs
    | TerranCommandCenter = 106

    // Zerg Building IDs
    | ZergHatchery = 131

    // Protos Building IDs
    | ProtosNexus = 154

    // ---- Misc ----
    // Misc Units
    | ZergLava = 35
    | CritterKakaru = 94

    // Resource IDs
    | MineralField  = 176
    | VespeneGeyser = 188

    | Unknown = -1

/// Correspond to the current orders for each unit provided by StarCraft
type UnitOrder =
    | Die                       = 000
    | Stop                      = 001
    | Guard                     = 002
    | PlayerGuard               = 003
    | TurretGuard               = 004
    | BunkerGuard               = 005
    | Move                      = 006
    | ReaverStop                = 007
    | Attack1                   = 008
    | Attack2                   = 009
    | AttackUnit                = 010
    | AttackFixedRange          = 011
    | AttackTile                = 012
    | Hover                     = 013
    | AttackMove                = 014
    | InfestMine1               = 015
    | Nothing1                  = 016
    | Powerup1                  = 017
    | TowerGuard                = 018
    | TowerAttack               = 019
    | VultureMine               = 020
    | StayinRange               = 021
    | TurretAttack              = 022
    | Nothing2                  = 023
    | Nothing3                  = 024
    | DroneStartBuild           = 025
    | DroneBuild                = 026
    | InfestMine2               = 027
    | InfestMine3               = 028
    | InfestMine4               = 029
    | BuildTerran               = 030
    | BuildProtoss1             = 031
    | BuildProtoss2             = 032
    | ConstructingBuilding      = 033
    | Repair1                   = 034
    | Repair2                   = 035
    | PlaceAddon                = 036
    | BuildAddon                = 037
    | Train                     = 038
    | RallyPoint1               = 039
    | RallyPoint2               = 040
    | ZergBirth                 = 041
    | Morph1                    = 042
    | Morph2                    = 043
    | BuildSelf1                = 044
    | ZergBuildSelf             = 045
    | Build5                    = 046
    | Enternyduscanal           = 007
    | BuildSelf2                = 448
    | Follow                    = 049
    | Carrier                   = 050
    | CarrierIgnore1            = 051
    | CarrierStop               = 052
    | CarrierAttack1            = 053
    | CarrierAttack2            = 054
    | CarrierIgnore2            = 055
    | CarrierFight              = 056
    | HoldPosition1             = 057
    | Reaver                    = 058
    | ReaverAttack1             = 059
    | ReaverAttack2             = 060
    | ReaverFight               = 061
    | ReaverHold                = 062
    | TrainFighter              = 063
    | StrafeUnit1               = 064
    | StrafeUnit2               = 065
    | RechargeShields1          = 066
    | Rechargeshields2          = 067
    | ShieldBattery             = 068
    | Return                    = 079
    | DroneLand                 = 070
    | BuildingLand              = 071
    | BuildingLiftoff           = 072
    | DroneLiftoff              = 073
    | Liftoff                   = 074
    | ResearchTech              = 075
    | Upgrade                   = 076
    | Larva                     = 077
    | SpawningLarva             = 078
    | Harvest1                  = 079
    | Harvest2                  = 080
    | MoveToGas                 = 081                  // Unit is moving to refinery 
    | WaitForGas                = 082                  // Unit is waiting to enter the refinery (another unit is currently in it) 
    | HarvestGas                = 083                  // Unit is in refinery 
    | ReturnGas                 = 084                  // Unit is returning gas to center 
    | MoveToMinerals            = 085                  // Unit is moving to mineral patch 
    | WaitForMinerals           = 086                  // Unit is waiting to use the mineral patch (another unit is currently mining from it) 
    | MiningMinerals            = 087                  // Unit is mining minerals from mineral patch 
    | Harvest3                  = 088                 
    | Harvest4                  = 089                 
    | ReturnMinerals            = 090                  // Unit is returning minerals to center 
    | Harvest5                  = 091
    | EnterTransport            = 092
    | Pickup1                   = 093
    | Pickup2                   = 094
    | Pickup3                   = 095
    | Pickup4                   = 096
    | Powerup2                  = 097
    | SiegeMode                 = 098
    | TankMode                  = 099
    | WatchTarget               = 100
    | InitCreepGrowth           = 101
    | SpreadCreep               = 102
    | StoppingCreepGrowth       = 103
    | GuardianAspect            = 104
    | WarpingArchon             = 105
    | CompletingArchonsummon    = 106
    | HoldPosition2             = 107
    | HoldPosition3             = 108
    | Cloak                     = 109
    | Decloak                   = 110
    | Unload                    = 111
    | MoveUnload                = 112
    | FireYamatoGun1            = 113
    | FireYamatoGun2            = 114
    | MagnaPulse                = 115
    | Burrow                    = 116
    | Burrowed                  = 117
    | Unburrow                  = 118
    | DarkSwarm                 = 119
    | CastParasite              = 120
    | SummonBroodlings          = 121
    | EmpShockwave              = 122
    | NukeWait                  = 123
    | NukeTrain                 = 124
    | NukeLaunch                = 125
    | NukePaint                 = 126
    | NukeUnit                  = 127
    | NukeGround                = 128
    | NukeTrack                 = 129
    | InitArbiter               = 130
    | CloakNearbyUnits          = 131
    | PlaceMine                 = 132
    | Rightclickaction          = 133
    | SapUnit                   = 134
    | SapLocation               = 135
    | HoldPosition4             = 136
    | Teleport                  = 137
    | TeleporttoLocation        = 138
    | PlaceScanner              = 139
    | Scanner                   = 140
    | DefensiveMatrix           = 141
    | PsiStorm                  = 142
    | Irradiate                 = 143
    | Plague                    = 144
    | Consume                   = 145
    | Ensnare                   = 146
    | StasisField               = 147
    | Hallucianation1           = 148
    | Hallucination2            = 149
    | ResetCollision1           = 150
    | ResetCollision2           = 151
    | Patrol                    = 152
    | CTFCOPInit                = 153
    | CTFCOP1                   = 154
    | CTFCOP2                   = 155
    | ComputerAI                = 156
    | AtkMoveEP                 = 157
    | HarassMove                = 158
    | AIPatrol                  = 159
    | GuardPost                 = 160
    | RescuePassive             = 161
    | Neutral                   = 162
    | ComputerReturn            = 163
    | InitPsiProvider           = 164
    | SelfDestrucing            = 165
    | Critter                   = 166
    | HiddenGun                 = 167
    | OpenDoor                  = 168
    | CloseDoor                 = 169
    | HideTrap                  = 170
    | RevealTrap                = 171
    | Enabledoodad              = 172
    | Disabledoodad             = 173
    | Warpin                    = 174
    | Medic                     = 175
    | MedicHeal1                = 176
    | HealMove                  = 177
    | MedicHoldPosition         = 178
    | MedicHeal2                = 179
    | Restoration               = 180
    | CastDisruptionWeb         = 181
    | CastMindControl           = 182
    | WarpingDarkArchon         = 183
    | CastFeedback              = 184
    | CastOpticalFlare          = 185
    | CastMaelstrom             = 186
    | JunkYardDog               = 187
    | Fatal                     = 188
    | None                      = 189
    | Unknown                   = 190
   

