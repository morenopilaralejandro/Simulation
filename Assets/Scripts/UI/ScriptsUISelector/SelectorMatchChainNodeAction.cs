using UnityEngine;
using Aremoreno.Enums.Match;

public class SelectorMatchChainNodeAction : ISelectorClickAction<MatchChainNode>
{
    public void Execute(MatchChainNode n, IClosableMenu menu)
    {
        switch (n)
        {
            case MatchChainNodeMatch nodeMatch:
                nodeMatch.Complete();
                break;

            default:
                UIEvents.RaiseMatchChainNodeDetailOpened(n);
                return;
        }
        //n.Complete();
    }
}
