=== inknpc_shop_quevedo ===
Hello #loc:greeting_what #speaker:chara-00125-quevedo #kit:kit-00004-enigma:away:field #mood:happy
+ Play match chain #loc:choice_road
    #cmd:open_menu:match_chain:match_chain-00002-park
    -> DONE
+ Buy #loc:choice_buy
    #cmd:open_menu:shop:shop-00009-move_a
    -> DONE
+ Sell #loc:choice_sell
    #cmd:open_menu:sell
    -> DONE
+ About you #loc:choice_about_you
    Text #loc:npc_ufoclub_about_you_0 #speaker:chara-00125-quevedo #kit:kit-00004-enigma:away:field #mood:happy
    -> inknpc_shop_quevedo
+ About this place #loc:choice_about_place
    Text #loc:npc_ufoclub_about_place_0 #speaker:chara-00125-quevedo #kit:kit-00004-enigma:away:field #mood:happy
    Text #loc:npc_ufoclub_about_place_1 #speaker:chara-00125-quevedo #kit:kit-00004-enigma:away:field #mood:happy
    -> inknpc_shop_quevedo
+ Goodbye #loc:choice_goodbye
    Come back soon #loc:farewell_suspense #speaker:chara-00125-quevedo #kit:kit-00004-enigma:away:field #mood:happy
    -> DONE
