using BusTrackerWeb.Models;
using BusTrackerWeb.Models.GoogleApi;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BusTrackerWeb.Controllers
{
    /// <summary>
    /// Controls all Bus Tracker Journey View actions.
    /// </summary>
    public class JourneyController : Controller
    {
        /// <summary>
        /// Open the Your Journey View.
        /// </summary>
        /// <returns>Your Journey View.</returns>
        public async Task<ActionResult> Index(int runId, int routeId)
        {
            ViewBag.Title = "BusHop > Your Journey";

            // Get the stopping pattern for the selected run.
            RouteModel departureRoute = new RouteModel { RouteId = routeId };
            RunModel departureRun = new RunModel { RunId = runId, Route = departureRoute };
            StoppingPatternModel pattern = await WebApiApplication.PtvApiControl.GetStoppingPatternAsync(departureRun);

            // Build an array of stops and pass to the view as marker coordinates.
            DepartureModel[] departureArray = pattern.Departures.ToArray();
            ViewBag.StopsLatitudeArray = new double[departureArray.Count()];
            ViewBag.StopsLongitudeArray = new double[departureArray.Count()];

            ViewBag.Departures = pattern.Departures;

            for (int i = 0; i < departureArray.Count(); i++)
            {
                ViewBag.StopsLatitudeArray[i] = (double)departureArray[i].Stop.StopLatitude;
                ViewBag.StopsLongitudeArray[i] = (double)departureArray[i].Stop.StopLongitude;
            }

            // Build and array of stop coordinates.
            List<GeoCoordinate> stopCoordinates = new List<GeoCoordinate>();
            foreach(DepartureModel departure in pattern.Departures)
            {
                stopCoordinates.Add(new GeoCoordinate((double)departure.Stop.StopLatitude, (double)departure.Stop.StopLongitude));
            }

            // Get directions between stops.
            List<Route> runRoutes = WebApiApplication.DirectionsApiControl.GetDirections(stopCoordinates.ToArray());
            List<string> polyLines = new List<string>();
            runRoutes.ForEach(r => polyLines.Add(r.overview_polyline.points.Replace(@"\\", @"\\\\")));
            
            ViewBag.EncodePolylines = polyLines.ToArray();

            return View("~/Views/Journey/Index.cshtml", pattern.Departures);
        }

        public ActionResult TestMap()
        {
            string[] polyLines = new string[2];

            // Geelong Deakin
            polyLines[0] = "ffygFwjapZR{@BY?OKBMBYDUREH@B?FENIDKCGK?Oo@a@QwE@m@zCiAtAcAv@w@x@s@j@Od@EHi@v@gEjAyFfE|ArNbFxDtAlE~At@N~Af@fHbCnBv@RHLN`@LnBt@r@V~Aj@tR`HbZjKtE`Bt@XhAl@rAfAb@d@rA`B~DjEpC|C`BhBdDlD~DlEnAvA~BfC`ClCfB`B~AfBf@j@n@x@v@v@v@x@PRj@j@LWb@k@~AqAbEyDnEaEzAuARG\\CjCXzI`AbAL`@H^C~@H\\CbDd@zC^zHz@z\\xDfE`@nFj@bDXd@HCVi@bI@??@@@@@?DADG@EIBIB?RcCV_EcEa@gKgAkAK_Fk@gCYYVGBOAqAOCd@QrC_AKE?C?I?QDOJKN}@dDS`@iAbAUHWDY?{AQCo@Ai@T}Dd@oIpUjCfGr@hGj@nEf@tBR[vEQbC@@@@@DCHIAAKFE@?h@cIcEa@sMsAoDa@OABYBg@Fg@Bw@FeARcBFqBF}ABQGEEEoAKsAGgAOaB[mAIy@CYCWfE]vFGnAfGr@rFl@jEf@dIv@nEf@tBRhGp@~Gv@`CXCVi@dJ_@dHu@dLoBnZCrAFbAF^n@tCZrAHn@D~@?`AEzADn@Lt@Z|@\\d@ZXb@Rx@PrCZvFn@";
            polyLines[1] = "hxchFi{{oZfIt@|@JR_BHe@Id@g@rD_AzG@^QnA]vB@@?@@B?FAHEBC@Gl@GJ]lBUv@[p@?H?FMXQd@gArA{BxBoC~CeCrCsBxB]ZMUr@w@zCeDrBiCpBsBlBcBfAuA`@y@BCF?L]Xs@Ru@RoA?IACHm@AAAACIBODCLBBLENC@CJEb@GHSnASt@Yr@M`@?HWt@s@`AcBbBqAtAiCxCuBbCo@p@CLOh@CZ@h@dEdIxCbGdI~Od@bAHBB@NTXh@RT`AWtAa@`Bc@hCO`@A~BNrCCHAtAOHGJ@DBlDN|KnA?ABA@AD?JBBBvMbBhAHBADEF?LH@DLDnCZ`BP`ALdD^bCXf@Hb@FXCDANBDLAPIJM?IIAC[IWEsBSmGs@aEc@sAOGDM?KM?SHMLCLJFDtBXtCZzFp@zBXb@F\\?DCFALFBNANI^Az@Bf@VdEBf@[DiAJe@CArLAnBDlAlBGz@Cx@KA[G{@Fz@ZjDBl@?p@@@@D?BAHEFKn@ENEt@s@lLKdA}QmBgAOiS{BcLiAqGw@}B[]SoAuAQMo@KI?_@`@x@pD^fAD@HDFHBZAJENKHU@KEGIEKMOU_@Oc@a@eBi@_DkAoHUc@Og@Mc@[_@a@Ja@?aA\\m@VcAVsAXcA\\gDf@y@Ju@JMNYt@?hB@@@@@D@JELEBEt@EL_@fFBpC^bI";

            //// Frankston - Portsea
            //polyLines[0] = "b}xgF_xwtZTF@A@CFAv@Fp@HB?D@FDDNAH?DrBzFf@pA@@@@DBDPCPKF]bAi@pAYhAGV?B?BAHCDg@tAi@fBxC`Bf@Xr@h@l@b@p@p@jC|C`A|Ad@nAdAhDj@fAn@t@n@d@zAbA`BdAtA~@pBdB`IhHrAvATd@l@tAr@hDXt@h@x@tApApAnAV\\Pb@Rf@Rv@Hp@B|DJlAZz@j@v@xBxBh@b@^Tt@VtFdBz@\\|@\\nCfA`M|E`C`ArCbAbEbBtBz@t@ZjA`@`APfALzBCtCMtBEbB@tAJnATlCz@fBz@pCjBfAv@v@^rD~Al@^p@j@x@z@Z^`BnCt@bAn@n@p@b@`A`@lAXjAJ|DEzAB|Db@fDr@xBj@E\\AJ?HDRJLn@x@b@|@Zr@V\\jLtOZd@FDHHf@hA?@B@@B?H?@xBlFn@zA\\d@^Vv@V`@H^BhAKv@Ez@K|@I\\BjCU~D]tCWd^}CdE_@lAK^Kh@WHEXYT[j@lAP`@fAxB`OjZfInPz@vAnAfBdOnSpBlCrHnKtUf\\tX``@xGxIvEfGnHhKfGpI~FlI|@nA`AnAOv@iA}AcDwEwB_DVa@Zd@fE~FxBxCpAfBlHfKtEpGhNvRdBbCh@p@bA`BlDpFlA~A";
            //polyLines[1] = "~lihF{lhtZtDvEpBhCuAz@gAh@aJ~EyOxIa@Tk@l@WTADCBIBCAA?q@NqAl@IEGIAMFIJC`@?LCl@WZM@C@EBCFAD?@@nAm@tAs@tEiClJgFhHaEZMh@]l@]NRtA|B|@vA~BdDtChEbFdHdHxJjDvEd@r@zBjCn@b@f@ZnAf@l@Nh@Dh@Bf@?h@AfAQzB[lHiAxE{@rF}@jEw@|BYrACz@ArAH~DZxDXdGd@`KtArGh@`RpAz@Bn@Hh@D`@Jp@NxEjAVNVDvAN\\@xBEzJi@jAKbAS`Aa@pBmAzB_BnAu@j@SrBe@dBWlAGl@BrAXdBj@~F`CxEbBfGdCtDvAnH`EzB`Ah@T`Dr@lBTz@BlA?jLi@nCEdBB~AJpC`@f]rH~DjAfAZfUdGrF~AvA^dBZxAJhBElB[fA]xAs@p@a@zEaD~CuBDHDDHB^?l@MjAa@dBc@tAS`BAbCPz@Pp@VdBx@nB~A~@hAnB~CbHzLp@tA\\~@Pr@ZjBJxA@pAQvDuAxUgB~[i@~J@\\@RFd@JZDFFL@NAPELOL_@r@I^Mx@u@xLAd@BZL`@dD`Hf@rAfBtIPj@BFD\\@Rd@rBFPBHBP?Rr@nDFl@@v@GzCKxFBf@Jp@`@zBBzCFjH@`ESrAw@dCE^BPLR\\Od@S`@Gj@ChADt@P~AJ|E`@rAXhATvGbBjBf@`AZdErBbC}IAAACAE@KHEHD@J?BjCvA";
            //polyLines[2] = "brzhFew}sZzIfH~@z@dQlPzEtEhDtFzK|QdE|GdMxSJJZHWfEDPCr@EfE?X?p@JxAX|ANh@?PlCbHtIbUtKfY~AfErAhDp@jBJFZr@\\~@pBnF@Xb@jAbAlCnClHbCxG`@`AnAvFbAnEz@`DzEnNvBpGr@pBpCxIfAjF|@jFj@dDd@nBpAhErAtFZ~@l@|A`AdBd@p@|@~AVj@xBjFjCzG~B|GnE|MnD`KbAdDvBlGfD|JvBjGjAlFrBbKdAvF\\nBnB~K|@tFJPtFd[t@fE`AxE`@`CFbABnBFp@XpB^`BR|@NxAbBhJz@xE^~AJXd@~@Xr@d@|BXzA";
            //polyLines[3] = "vubiFsnksZxAdIvUJCvDGJODkRMWBSHMq@eAuF]cA{@wBWgAI]z@[nBnKv@jEnEhV`B|IxCbP`@`CDf@ZbBp@rDdBnJ~Gn_@RjAL|@rAvIbBlKb@dDHPj@rDb@tChAzGPlA?RJh@lFl\\t@|El@~DnBxNxBzP|@tGdBnNdAvLdAxNjD~d@^zFFfBDrFHpJKBKHIVATDZ@Z?f@FLDBPDDlGDhF@pAFhHFpGBtJ?~CGvAg@`Iq@`K}@tN";
            //polyLines[4] = "pceiFmd|rZ@MNQj@kJHiAIhAk@jJ@XSzC]|E_@zEABGJ_@tDg@bEiAjJi@dEW`AkC|Iu@tCS~@c@~CJDJBKCKEa@jDg@nEWvBiA|JuCbTmArIwA~Js@xF{Epa@e@rD[jBwBhK{BrK{@fFo@~EiAdIm@pHmAjOStAQjA{@nDgAfF{BvJ{B|IyBhIiAtDgB|Fk@dBo@~AgAvBoE`Hm@t@{@v@_Ah@aAZgBZkALYDUAgGz@eBTqF`@yAd@c@T}@p@q@t@u@lAg@vAU`AWnB_@`DWxAm@lBm@jAgB`C`@h@rIxLZg@HWFi@?c@Im@MYwCgEa@c@]Q_@Km@C_@HYRIJa@i@tBuC^w@Z{@b@kBr@kG^gBf@wAR_@|@mAv@q@j@a@r@Yx@UpFa@|Eo@nCa@RIpAQvAUrAa@LGDPVz@Vf@@FbBbCfFjHdFfHhBhCr@l@@?@?B@DBDD@NELKFMCGO?Ga@cAaEwF_IaLyCgEKG_@k@SOe@OMBgBZkALYDUAgGz@eBTqDX_AFy@Ts@Xk@`@w@p@}@lAS^g@vAU`AWnB_@`DWxAm@lBm@jAaCdDsAtB[p@qAnDqCpI]hAoCbIcB|Em@tAe@v@a@f@v@|@rCbDfAzAn@`AjB|C";
            //polyLines[5] = "rl`iFwkmrZ~BhDtArAtAlAf@j@l@t@KPqDxF}BhD_A~@sAv@qG~CSk@m@y@{CeEaCeDoEvBuFfCaChA`ApGlAjHBBBBBD?LELKDMAQOAUBEUiBe@}DPElAjHBBBBBD?LELKDMAKGCCYDm@NICm@XiAd@oDdBkLpFQPULW\\?FCHMDC?OL_@^Uh@Oj@mBvNcAvHeBtMmAzJ}B|PkEv\\WrBM|AeC[kAMqEi@gBWcSeCs]kEyAvTgAnPk@|KShEk@xIw@jJMvA@fA?`@GPWGGGXwA";

            for (int i = 0; i < polyLines.Count(); i++)
            {
                polyLines[i] = polyLines[i].Replace(@"\\", @"\\\\");
            }

            ViewBag.EncodePolylines = polyLines.ToArray();

            return View("~/Views/Journey/TestMap.cshtml");
        }

    }
}
