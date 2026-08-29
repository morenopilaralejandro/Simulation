=== inknpc_boss_train_0 ===
Text #loc:npc_male_villain_derail_0 #speaker:chara-00040-malaki #kit:kit-00002-crimson:home:field #mood:happy
+ About you #loc:choice_about_you
    Text #loc:npc_male_villain_derail_1 #speaker:chara-00040-malaki #kit:kit-00002-crimson:home:field #mood:happy
    -> inknpc_boss_train_0
+ Unforgivable #loc:choice_unforgivable
    Text #loc:npc_unforgivable_0 #speaker:chara-00040-malaki #kit:kit-00002-crimson:home:field #mood:happy
    ~ SetGameFlag("pending_boss_dam", false)
    ~ SetGameFlag("pending_boss_train", true)
    ~ SetGameFlag("pending_boss_power_plant", false)
    ~ SetGameFlag("pending_boss_volcano", false)
    ~ SetGameFlag("pending_teleporter", false)
    #cmd:transition:zone_interior_realm_evil:spawn_realm_evil_0_default
    -> DONE
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_out #speaker:chara-00040-malaki #kit:kit-00002-crimson:home:field #mood:happy
    -> DONE
