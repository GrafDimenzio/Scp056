﻿using System.Linq;
using Neuron.Core.Events;
using Neuron.Core.Meta;
using Ninject;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using Synapse3.SynapseModule;
using Synapse3.SynapseModule.Events;
using Synapse3.SynapseModule.Player;
using UnityEngine;
using VoiceChat;

namespace Scp056;

[Automatic]
public class EventHandler : Listener
{
    [Inject]
    public Scp056Plugin Plugin { get; set; }
    [Inject]
    public PlayerService Player { get; set; }

    [EventHandler]
    public void FirstSpawn(FirstSpawnEvent ev)
    {
        if (!Plugin.Config.EnableDefaultSpawnBehaviour) return;
        if (Player.Players.Count < Plugin.Config.RequiredPlayers) return;
        if (Random.Range(1f, 100f) > Plugin.Config.SpawnChance) return;
        if(Plugin.Config.ReplaceScp && ev.AmountOfScpSpawns <= 0) return;

        var possiblePlayers = Player.Players.Where(x => RoleAssigner.CheckPlayer(x.Hub)).ToArray();
        if (!possiblePlayers.Any()) return;
        var player = possiblePlayers[Random.Range(0, possiblePlayers.Count())];
        ev.PlayersBlockedFromSpawning.Add(player);
        player.RoleID = 56;
        if (Plugin.Config.ReplaceScp)
            ev.AmountOfScpSpawns--;
    }
    
    [EventHandler]
    public void Death(DeathEvent ev)
    {
        if (ev.Attacker == null || ev.Attacker == ev.Player) return;

        if (ev.Player.RoleID == 56)
            ev.Attacker.SendBroadcast(Plugin.Translation.Get(ev.Attacker).Killed056, 7);
        else if (ev.Attacker.RoleID == 56)
            ev.Player.SendWindowMessage(Plugin.Translation.Get(ev.Player).KilledBy056);
    }

    //TODO: Implement SCP Chat ability
    /*
    [EventHandler]
    public void SpeakEvent(SpeakEvent ev)
    {
        if (ev.Player.RoleID == 56 && (ev.Player.CustomRole as Scp056PlayerScript)?.ScpChat == true)
        {
            SynapseLogger<Scp056Plugin>.Warn("SpeakEvent");
            ev.Channel = VoiceChatChannel.ScpChat;
        }
    }

    [EventHandler]
    public void SpeakPlayerEvent(SpeakToPlayerEvent ev)
    {
        if (ev.Receiver.RoleID == 56 && ev.Player.Hub.IsSCP() &&
            (ev.Player.CustomRole as Scp056PlayerScript)?.ScpChat == true)
        {
            SynapseLogger<Scp056Plugin>.Warn("SpeakEventPlayer");
            ev.Channel = VoiceChatChannel.ScpChat;
        }
    }
    */
}