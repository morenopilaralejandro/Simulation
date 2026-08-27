=== inknpc_shop_pichi ===
Hello #loc:greeting_how #speaker:chara-00031-pichi #kit:kit-00003-balance:home:field #mood:happy
+ Buy #loc:choice_buy
    #cmd:open_menu:shop:shop-00006-equiment_b
    -> DONE
+ Sell #loc:choice_sell
    #cmd:open_menu:sell
    -> DONE
+ About you #loc:choice_about_you
    Text #loc:npc_pichi_you_0 #speaker:chara-00031-pichi #kit:kit-00003-balance:home:field #mood:happy
    Text #loc:npc_pichi_you_1 #speaker:chara-00031-pichi #kit:kit-00003-balance:home:field #mood:happy
    -> inknpc_shop_pichi
+ About this place #loc:choice_about_place
    Text #loc:npc_pichi_place_0 #speaker:chara-00031-pichi #kit:kit-00003-balance:home:field #mood:happy
    -> inknpc_shop_pichi
+ Goodbye #loc:choice_goodbye
    Come back soon #loc:farewell_farewell #speaker:chara-00031-pichi #kit:kit-00003-balance:home:field #mood:happy
    -> DONE
