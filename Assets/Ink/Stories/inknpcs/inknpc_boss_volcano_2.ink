=== inknpc_boss_volcano_2 ===
Text #loc:npc_villain_volcano_shard_0 #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
Text #loc:item_shard_anger_0 #speaker:system  #sfx:sfx-chest_get_item
Text #loc:npc_villain_volcano_shard_1 #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
~ GiveItem("item-important-00004-shard_anger", 1)
~ SetGameFlag("get_shard_volcano", true)
~ SetGameFlag("pending_teleporter", true)
~ SetGameFlag("allow_quick_travel", true)
~ SetGameFlag("pending_boss_volcano", false)
#cmd:transition:zone_interior_saint_justice_entrance:spawn_saint_justice_checkpoint_boru
-> DONE
