﻿namespace AnyZote;

public partial class Control : Module
{
    private void LoadPrefabsRoar(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        var battleControl = preloadedObjects["GG_Mighty_Zote"]["Battle Control"];
        var names = new List<(string, string, string)> {
            ("Zote Thwomp", "", "Thwomp Zoteling"),
            ("Zote Fluke", "", "Fluke Zoteling"),
            ("Fat Zotes", "Zote Crew Fat (1)", "Fat Zoteling"),
            ("Zote Salubra", "", "Salubra Zoteling"),
        };
        foreach ((string group, string instance, string name) in names)
        {
            GameObject minion;
            if (instance != "")
            {
                minion = battleControl.transform.Find(group).gameObject.transform.Find(instance).gameObject;
            }
            else
            {
                minion = battleControl.transform.Find(group).gameObject;
            }
            UnityEngine.Object.Destroy(minion.GetComponent<PersistentBoolItem>());
            UnityEngine.Object.Destroy(minion.GetComponent<ConstrainPosition>());
            prefabs[name] = minion;
        }
    }
    private void UpdateFSMRoar(PlayMakerFSM fsm)
    {
        if (IsGreyPrince(fsm.gameObject) && fsm.FsmName == "Control")
        {
            fsm.AddState("Roar Check");
            UpdateStateRoarCheck(fsm);
            fsm.InsertCustomAction("Spit Antic", () =>
            {
                fsm.FsmVariables.FindFsmInt("ActiveZotelings").Value = 0;
            }, 2);
            void Spit(PlayMakerFSM fsm)
            {
                var zoteling = fsm.FsmVariables.FindFsmGameObject("Zoteling").Value;
                zoteling.GetComponent<Renderer>().enabled = false;
                zoteling.RemoveComponent<DamageHero>();
                zoteling.RemoveComponent<BoxCollider2D>();
                var minion = toSpit[0];
                toSpit.RemoveAt(0);
                minion = UnityEngine.Object.Instantiate(minion);
                minion.SetActive(true);
                minion.SetActiveChildren(true);
                minion.transform.position = zoteling.transform.position;
                if (minion.name.StartsWith("Zote Salubra"))
                {
                    minion.GetComponent<Rigidbody2D>().velocity = 0.5f * zoteling.GetComponent<Rigidbody2D>().velocity;
                }
                else
                {
                    minion.GetComponent<Rigidbody2D>().velocity = zoteling.GetComponent<Rigidbody2D>().velocity;
                }
            }
            fsm.RemoveAction("Spit L", 7);
            fsm.AddCustomAction("Spit L", () => Spit(fsm));
            fsm.RemoveAction("Spit R", 7);
            fsm.AddCustomAction("Spit R", () => Spit(fsm));
            fsm.InsertCustomAction("Respit?", () =>
            {
                if (toSpit.Count > 0)
                {
                    fsm.SendEvent("REPEAT");
                }
                else
                {
                    fsm.SendEvent("FINISHED");
                }
            }, 0);
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name.StartsWith("Zote Balloon ") && fsm.FsmName == "Control")
        {
            fsm.AddCustomAction("Set Pos", () =>
            {
                GameObject minion;
                if (random.Next(2) == 0)
                {
                    minion = prefabs["Thwomp Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 23.4f, fsm.gameObject.transform.position.z);
                    minion.transform.SetScaleX(0.5f * minion.transform.localScale.x);
                    minion.transform.SetScaleY(0.5f * minion.transform.localScale.y);
                    minion.transform.SetScaleZ(0.5f * minion.transform.localScale.z);
                    fsm.SetState("Dormant");
                }
                var greyPrince = GameObject.Find("Grey Prince");
                if (greyPrince.LocateMyFSM("Control").AccessIntVariable("wave3Cnt").Value > 0)
                {
                    greyPrince.LocateMyFSM("Control").AccessIntVariable("wave3Cnt").Value -= 1;
                    minion = prefabs["Salubra Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 10, fsm.gameObject.transform.position.z);
                    minion.GetComponent<PlayMakerFSM>().AddCustomAction("Idle", () =>
                    {
                        var voice = minion.transform.Find("Voice").gameObject;
                        voice.SetActive(false);
                    });
                    var ghostMovement = minion.GetComponent<PlayMakerFSM>().GetAction<GhostMovement>("Sucking", 8);
                    ghostMovement.yPosMin = ghostMovement.yPosMin.Value + 5;
                    ghostMovement.yPosMax = ghostMovement.yPosMax.Value + 5;
                }
                else
                {
                    minion = prefabs["Fluke Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 6, fsm.gameObject.transform.position.z);
                    minion.transform.SetScaleX(0.5f * minion.transform.localScale.x);
                    minion.transform.SetScaleY(0.5f * minion.transform.localScale.y);
                    minion.transform.SetScaleZ(0.5f * minion.transform.localScale.z);
                }
            });
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name.StartsWith("Zote Balloon") && fsm.FsmName == "Control")
        {
            fsm.AddCustomAction("Set Pos", () =>
            {
                GameObject minion;
                if (random.Next(2) == 0)
                {
                    minion = prefabs["Thwomp Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 23.4f, fsm.gameObject.transform.position.z);
                    minion.transform.SetScaleX(0.5f * minion.transform.localScale.x);
                    minion.transform.SetScaleY(0.5f * minion.transform.localScale.y);
                    minion.transform.SetScaleZ(0.5f * minion.transform.localScale.z);
                    fsm.SetState("Dormant");
                }
                var greyPrince = GameObject.Find("Grey Prince");
                if (greyPrince.LocateMyFSM("Control").AccessIntVariable("wave3Cnt").Value > 0)
                {
                    greyPrince.LocateMyFSM("Control").AccessIntVariable("wave3Cnt").Value -= 1;
                    minion = prefabs["Salubra Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 10, fsm.gameObject.transform.position.z);
                    minion.GetComponent<PlayMakerFSM>().AddCustomAction("Idle", () =>
                    {
                        var voice = minion.transform.Find("Voice").gameObject;
                        voice.SetActive(false);
                    });
                    var ghostMovement = minion.GetComponent<PlayMakerFSM>().GetAction<GhostMovement>("Sucking", 8);
                    ghostMovement.yPosMin = ghostMovement.yPosMin.Value + 5;
                    ghostMovement.yPosMax = ghostMovement.yPosMax.Value + 5;
                }
                else
                {
                    minion = prefabs["Fluke Zoteling"] as GameObject;
                    minion = UnityEngine.Object.Instantiate(minion);
                    minion.SetActive(true);
                    minion.SetActiveChildren(true);
                    minion.transform.position = new Vector3(fsm.gameObject.transform.position.x, 6, fsm.gameObject.transform.position.z);
                    minion.transform.SetScaleX(0.5f * minion.transform.localScale.x);
                    minion.transform.SetScaleY(0.5f * minion.transform.localScale.y);
                    minion.transform.SetScaleZ(0.5f * minion.transform.localScale.z);
                }
                minion = prefabs["Thwomp Zoteling"] as GameObject;
                minion = UnityEngine.Object.Instantiate(minion);
                minion.SetActive(true);
                minion.SetActiveChildren(true);
                minion.transform.position = new Vector3(26.4f + (float)(1 - random.NextDouble() * 2) * 10, 23.4f, fsm.gameObject.transform.position.z + 1e-2f);
                var previousX = minion.transform.position.x;
                minion.transform.SetScaleX(1f * minion.transform.localScale.x);
                minion.transform.SetScaleY(1f * minion.transform.localScale.y);
                minion.transform.SetScaleZ(1f * minion.transform.localScale.z);
                minion = prefabs["Fluke Zoteling"] as GameObject;
                minion = UnityEngine.Object.Instantiate(minion);
                minion.SetActive(true);
                minion.SetActiveChildren(true);
                minion.transform.position = new Vector3(previousX, 6, fsm.gameObject.transform.position.z + 1e-2f);
                minion.transform.SetScaleX(1f * minion.transform.localScale.x);
                minion.transform.SetScaleY(1f * minion.transform.localScale.y);
                minion.transform.SetScaleZ(1f * minion.transform.localScale.z);
            });
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name == "Zote Thwomp(Clone)" && fsm.FsmName == "Control")
        {
            fsm.AddTransition("Dormant", "FINISHED", "Set Pos");
            fsm.InsertCustomAction("Set Pos", () =>
            {
                fsm.FsmVariables.GetFsmFloat("X Pos").Value = fsm.gameObject.transform.position.x;
                fsm.gameObject.transform.Find("Enemy Crusher").gameObject.SetActive(false);
            }, 1);
            fsm.RemoveAction("Set Pos", 2);
            fsm.RemoveAction("Slam", 5);
            fsm.RemoveAction("Down", 6);
            fsm.InsertCustomAction("Waves", () =>
            {
                fsm.SendEvent("FINISHED");
            }, 0);
            fsm.RemoveAction("Rise", 7);
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name == "Zote Fluke(Clone)" && fsm.FsmName == "Control")
        {
            Log("Upgrading FSM: " + fsm.gameObject.name + " - " + fsm.FsmName + ".");
            fsm.AddTransition("Dormant", "FINISHED", "Pos");
            fsm.InsertCustomAction("Pos", () =>
            {
                fsm.FsmVariables.GetFsmFloat("X Pos").Value = fsm.gameObject.transform.position.x;
            }, 1);
            fsm.RemoveAction("Pos", 4);
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name == "Zote Crew Fat (1)(Clone)" && fsm.FsmName == "Control")
        {
            Log("Upgrading FSM: " + fsm.gameObject.name + " - " + fsm.FsmName + ".");
            fsm.AddTransition("Dormant", "FINISHED", "Multiply");
            fsm.RemoveAction("Spawn Antic", 1);
            fsm.RemoveAction("Spawn Antic", 3);
            fsm.RemoveAction("Spawn Antic", 5);
            fsm.AddCustomAction("Spawn Antic", () => fsm.SendEvent("FINISHED"));
            fsm.RemoveAction("Tumble Out", 2);
            fsm.RemoveAction("Dr", 1);
            fsm.AddCustomAction("Dr", () =>
            {
                if (fsm.gameObject.transform.position.x < HeroController.instance.transform.position.x)
                {
                    fsm.SendEvent("R");
                }
                else
                {
                    fsm.SendEvent("L");
                }
            });
            fsm.AddCustomAction("Death Reset", () =>
            {
                UnityEngine.Object.Destroy(fsm.gameObject);
            });
        }
        else if (fsm.gameObject.scene.name == "GG_Grey_Prince_Zote" && fsm.gameObject.name == "Zote Salubra(Clone)" && fsm.FsmName == "Control")
        {
            Log("Upgrading FSM: " + fsm.gameObject.name + " - " + fsm.FsmName + ".");
            fsm.AddTransition("Dormant", "FINISHED", "Appear");
            fsm.RemoveAction("Appear", 3);
            fsm.RemoveAction("Appear", 5);
            fsm.RemoveAction("Idle", 0);
            fsm.RemoveAction("Idle", 0);
            var ghostMovement = FsmUtil.GetAction<HutongGames.PlayMaker.Actions.GhostMovement>(fsm, "Sucking", 8);
            ghostMovement.xPosMin = 9;
            ghostMovement.xPosMax = 44;
            fsm.RemoveAction("Dead", 1);
            fsm.AddCustomAction("Dead", () =>
            {
                UnityEngine.Object.Destroy(fsm.gameObject);
            });
        }

    }
    private void UpdateStateRoarCheck(PlayMakerFSM fsm)
    {
        fsm.AddCustomAction("Roar Check", () =>
        {
            var gameObject = GameObject.Find("Zote Thwomp(Clone)");
            if (gameObject != null)
            {
                fsm.SetState("Move Choice 3");
            }
            else
            {
                fsm.SetState("B Roar Antic");
            }
        });
    }
}