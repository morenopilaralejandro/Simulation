=== inknpc_priest ===
Hello #loc:greeting_holy #speaker:npc-00004-priest #mood:happy
+ Confess #loc:choice_confess
    #cmd:full_heal_all
    Text #loc:npc_priest_confess_1 #mood:happy
    -> DONE
+ Goodbye #loc:choice_goodbye
    Text #loc:farewell_holy #speaker:npc-00004-priest #mood:happy
    -> DONE
