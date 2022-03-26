using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MetaPosition : MetaBehaviour {

    protected override void OnReady(int userId) {}

    protected override void OnQuit(int userId) {}

    void Update() {
        Send(@"{
            ""evt"": ""position"",
            ""id"":"+GetInstanceID()+@",
            ""position"": {
                ""x"":"+transform.position.x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)+@",
                ""y"":"+transform.position.y.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)+@",
                ""z"":"+transform.position.z.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)+@"
            }
        }");
    }
}
