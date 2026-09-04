=== inknpc_boss_dam_2 ===
Text #loc:npc_villain_flood_shard_0 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
Text #loc:item_shard_sorrow_0 #speaker:system  #sfx:sfx-chest_get_item
Text #loc:npc_villain_flood_shard_1 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
~ GiveItem("item-important-00001-shard_sorrow", 1)
~ SetGameFlag("get_shard_dam", true)
~ SetGameFlag("pending_teleporter", true)
~ SetGameFlag("allow_quick_travel", true)
~ SetGameFlag("pending_boss_dam", false)
#cmd:transition:zone_interior_saint_justice_entrance:spawn_saint_justice_checkpoint_boru
-> DONE
