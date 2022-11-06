using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic.Teams
{
    [CreateAssetMenu(menuName = "Create TeamsConfiguration", fileName = "TeamsConfiguration", order = 1)]
    public class TeamsConfiguration : ScriptableObject
    {
        [SerializeField] private List<TeamComponent> _teamData;

        public int TotalTeams => _teamData.Count;
        
        public TeamComponent GetDataByIndex(int teamIndex) => _teamData.First(data => data.TeamId == teamIndex);
    }
}