namespace Phoenix.Core.Logging
{
    public class Heartbeat
    {
    //    public void ApplicationBeat(int appId, string appName)
    //    {
    //        using (PhoenixDataEntities pde = new PhoenixDataEntities())
    //        {
    //            var heartbeat = (from hb in pde.ApplicationHeartbeats
    //                             where hb.ApplicationId == 1
    //                             select hb).FirstOrDefault();
    //            if (heartbeat == null)
    //            {
    //                heartbeat = new ApplicationHeartbeat()
    //                {
    //                    ApplicationId = appId,
    //                    ApplicationName = appName,
    //                    LastStartUpTime = DateTime.Now,
    //                    LastCheckInTime = DateTime.Now
    //                };

    //                pde.AddToApplicationHeartbeats(heartbeat);
    //            }
    //            else
    //            {
    //                heartbeat.LastCheckInTime = DateTime.Now;
    //            }
    //            pde.SaveChanges();
    //        }
    //    }

    //    public void ApplicationOff(int appId)
    //    {
    //        using (PhoenixDataEntities pde = new PhoenixDataEntities())
    //        {
    //            var heartbeat = (from hb in pde.ApplicationHeartbeats
    //                             where hb.ApplicationId == appId
    //                             select hb).First();
    //            if (heartbeat != null)
    //            {
    //                pde.DeleteObject(heartbeat);
    //                pde.SaveChanges();
    //            }
    //        }     
    //    }
    }
}
