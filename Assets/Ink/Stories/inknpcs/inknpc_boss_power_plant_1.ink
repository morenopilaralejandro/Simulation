=== inknpc_boss_power_plant_1 ===
Text #loc:greeting_prepare #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
+ Play match #loc:choice_match
    #cmd:open_menu:match:match-00023-boss_2
    -> DONE
+ About this place #loc:choice_about_place
    Text #loc:npc_place_cube_0 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    Text #loc:npc_place_cube_1 #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    -> inknpc_boss_power_plant_1
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_out #speaker:chara-00124-inquina #kit:kit-00002-crimson:home:field #mood:happy
    -> DONE
