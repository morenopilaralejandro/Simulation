=== inknpc_boss_dam_0 ===
Text #loc:npc_boss_dam_greeting_0 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
+ About you #loc:choice_about_you
    Text #loc:npc_boss_dam_you_0 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
    Text #loc:npc_boss_dam_you_1 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
    -> inknpc_boss_dam_0
+ Unforgivable #loc:choice_unforgivable
    Text #loc:npc_unforgivable_0 #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
    ~ SetGameFlag("pending_boss_dam", true)
    ~ SetGameFlag("pending_teleporter", false)
    ~ SetGameFlag("allow_quick_travel", false)
    #cmd:transition:zone_interior_realm_evil:spawn_realm_evil_0_default
    -> DONE
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_out #speaker:chara-00010-ali #kit:kit-00002-crimson:home:keeper #mood:happy
    -> DONE
