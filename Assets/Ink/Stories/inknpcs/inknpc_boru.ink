=== inknpc_boru ===
Hello #loc:greeting_hello #speaker:npc-00001-boru #mood:happy
+ Play match chain #loc:choice_road
    #cmd:open_menu:match_chain:match_chain-00001-justice
    -> DONE
+ About you #loc:choice_about_you
    Text #loc:npc_angel_help_0 #speaker:npc-00001-boru #mood:happy
    -> inknpc_boru
+ About this place #loc:choice_about_place
    Text #loc:npc_angel_simulation_0 #speaker:npc-00001-boru #mood:happy
    Text #loc:npc_angel_debug_0 #speaker:npc-00001-boru #mood:happy
    Text #loc:npc_angel_quest_0 #speaker:npc-00001-boru #mood:happy
    Text #loc:npc_angel_quest_1 #speaker:npc-00001-boru #mood:happy
    -> inknpc_boru
+ Goodbye #loc:choice_goodbye
    Come back soon #loc:farewell_suspense #speaker:npc-00001-boru #mood:happy
    -> DONE
