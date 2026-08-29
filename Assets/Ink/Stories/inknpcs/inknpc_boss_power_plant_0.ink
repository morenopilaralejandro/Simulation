=== inknpc_boss_power_plant_0 ===
Text #loc:npc_female_villain_blackout_0 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
+ About you #loc:choice_about_you
    Text #loc:npc_female_villain_blackout_1 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    Text #loc:npc_female_villain_blackout_2 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    -> inknpc_boss_power_plant_0
+ Unforgivable #loc:choice_unforgivable
    Text #loc:npc_unforgivable_0 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    ~ SetGameFlag("pending_boss_dam", false)
    ~ SetGameFlag("pending_boss_train", false)
    ~ SetGameFlag("pending_boss_power_plant", true)
    ~ SetGameFlag("pending_boss_volcano", false)
    ~ SetGameFlag("pending_teleporter", false)
    #cmd:transition:zone_interior_realm_evil:spawn_realm_evil_0_default
    -> DONE
+ About this place #loc:choice_about_place
    Text #loc:npc_female_villain_ritual_0 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    -> inknpc_boss_power_plant_0
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_out #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    -> DONE
