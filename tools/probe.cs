// Scratch script: a one-line state report of the running game (Play Mode only).
var flow = UnityEngine.Object.FindAnyObjectByType<Noted.Game.GameFlow>();
if (flow == null) return "no GameFlow";
var s = flow.Session;
var player = flow.PlayerTransform;
var cards = UnityEngine.Object.FindAnyObjectByType<Noted.Game.CardScreen>();
var sb = new System.Text.StringBuilder();
sb.Append("phase=").Append(flow.Phase);
sb.Append(" frame=").Append(UnityEngine.Time.frameCount);
sb.Append(" cursor=").Append(UnityEngine.Cursor.lockState);
if (cards != null) sb.Append(" cardOpen=").Append(cards.IsOpen);
if (s != null)
{
    sb.Append(" day=").Append(s.Day);
    sb.Append(" rank=").Append(s.CurrentRank);
    sb.Append(" wallet=").Append(s.Wallet.Balance);
    sb.Append(" outstanding=").Append(s.CoinsOutstanding);
    if (s.Clock != null) sb.Append(" clock=").Append(s.Clock.Remaining.ToString("0.0"));
    sb.Append(" tasks=[");
    foreach (var t in s.TodaysTasks) sb.Append(t.Definition.Id).Append(":").Append(t.Status).Append(t.IsFinished ? "" : "@" + t.StepIndex).Append(' ');
    sb.Append("]");
    sb.Append(" carrying=[").Append(string.Join(",", s.Carried)).Append("]/").Append(s.CarryCapacity);
}
if (player != null) sb.Append(" pos=").Append(player.position.ToString("0.0"));
return sb.ToString();
