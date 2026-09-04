=== inknpc_boss_volcano_1 ===
Text #loc:greeting_prepare #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
+ Play match #loc:choice_match
    #cmd:open_menu:match:match-00024-boss_3
    -> DONE
+ About this place #loc:choice_about_place
    Text #loc:npc_place_cube_0 #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
    Text #loc:npc_place_cube_1 #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
    -> inknpc_boss_volcano_1
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_out #speaker:chara-00030-nervor #kit:kit-00002-crimson:home:field #mood:happy
    -> DONE
