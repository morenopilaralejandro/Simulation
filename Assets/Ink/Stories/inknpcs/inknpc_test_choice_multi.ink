=== inknpc_test_choice_multi ===
Hello #loc:test1 #speaker:chara-00001-are #kit:default #mood:happy
+ About you #loc:test1 #speaker:chara-00001-are #kit:default #mood:happy
    Text #loc:test1 #speaker:chara-00001-are #kit:default #mood:happy
    -> inknpc_test_choice_multi
+ About this place #loc:test2 #speaker:chara-00001-are #kit:default #mood:happy
    Text #loc:test2 #speaker:chara-00001-are #kit:default #mood:happy
    -> inknpc_test_choice_multi
+ Play match chain #loc:test1
    #cmd:open_menu:match_chain:match_chain-00001-park
    -> DONE
+ Buy #loc:test1
    #cmd:open_menu:shop:shop-00000-test
    -> DONE
+ Sell #loc:test2
    #cmd:open_menu:sell
    -> DONE
+ Goodbye #loc:test1
    Come back soon #loc:test1 #speaker:chara-00001-are #kit:default #mood:happy
    -> DONE
