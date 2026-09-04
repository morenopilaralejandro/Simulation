=== inknpc_shop_tieman ===
Hello #loc:greeting_suspense #speaker:npc-00005-villain #mood:happy
+ Play match chain #loc:choice_road
    #cmd:open_menu:match_chain:match_chain-00003-death
    -> DONE
+ Buy #loc:choice_buy
    #cmd:open_menu:shop:shop-00008-equiment_d
    -> DONE
+ Sell #loc:choice_sell
    #cmd:open_menu:sell
    -> DONE
+ About you #loc:choice_about_you
    Text #loc:npc_tieman_you_0 #speaker:npc-00005-villain #mood:happy
    -> inknpc_shop_tieman
+ Goodbye #loc:choice_goodbye
    Come back soon #loc:farewell_suspense #speaker:npc-00005-villain #mood:happy
    -> DONE
