=== inknpc_boss_power_plant_2 ===
Text #loc:npc_villain_blackout_shard_0 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
Text #loc:item_shard_hatred_0 #speaker:system  #sfx:sfx-chest_get_item
Text #loc:npc_villain_blackout_shard_1 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
~ GiveItem("item-important-00003-shard_hatred", 1)
~ SetGameFlag("get_shard_power_plant", true)
~ SetGameFlag("pending_teleporter", true)
~ SetGameFlag("allow_quick_travel", true)
~ SetGameFlag("pending_boss_power_plant", false)
#cmd:transition:zone_interior_saint_justice_entrance:spawn_saint_justice_checkpoint_boru
-> DONE
