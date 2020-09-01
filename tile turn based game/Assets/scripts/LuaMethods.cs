using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;
using System.Reflection;
using System;
using TileGame;
using System.Linq;
using System.IO;

namespace TileGame
{

    [MoonSharpUserData]
    public class LuaMethods : MonoBehaviour
    {
        public static LuaMethods instance = null;
        private DatabaseController DBC;
        private GameControllerScript GCS;
        private PlaySceneCamController PSCC;
        private MapEditMenueCamController MEMCC;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            try { 
            DBC = DatabaseController.instance;
            GCS = GameControllerScript.instance;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void SetLuaGlobalsAndTypes(Script script, string scene)
        {
            try { 
            //class
            UserData.RegisterType<GameObject>();
            UserData.RegisterType<AudioClip>();
            UserData.RegisterType<Building>();
            UserData.RegisterType<BuildingController>();
            UserData.RegisterType<ButtonProperties>();
            UserData.RegisterType<Camera>();
            UserData.RegisterType<Canvas>();
            UserData.RegisterType<CharacterController>();
            UserData.RegisterType<DatabaseController>();
            UserData.RegisterType<Debug>();
            UserData.RegisterType<Transform>();
            UserData.RegisterType<SpriteRenderer>();
            UserData.RegisterType<Component>();
            UserData.RegisterType<UnitController>();
            UserData.RegisterType<TerrainController>();

            //struc
            UserData.RegisterType<Vector2>();
            UserData.RegisterType<Vector3>();
            UserData.RegisterType<Color>();

            DynValue vec = UserData.Create(new Vector2());
            DynValue obj = UserData.Create(new GameObject());
            DynValue sprrend = UserData.Create(new SpriteRenderer());
            DynValue dbg = UserData.Create(new Debug());
            DynValue color = UserData.Create(new Color());
            DynValue comp = UserData.Create(new Component());
            DynValue unitcont = UserData.Create(new UnitController());
            DynValue terraincont = UserData.Create(new TerrainController());

            script.Globals.Set("Vector2", vec);
            script.Globals.Set("GameObject", obj);
            script.Globals.Set("SpriteRenderer", sprrend);
            script.Globals.Set("Debug", dbg);
            script.Globals.Set("Color", color);
            script.Globals.Set("Component", comp);
            script.Globals.Set("UnitController", unitcont);
            script.Globals.Set("TerrainController", terraincont);
            //UserData.RegisterProxyType<myProx, Color.>(r => new myProx(r)); //this is for classes

            script.Globals["GCS"] = GCS;
            script.Globals["LM"] = LuaMethods.instance;

            if (scene == "MapEditorScene")
            {
                MEMCC = GameObject.Find("MainCamera").GetComponent<MapEditMenueCamController>();
                script.Globals["MEMCC"] = MEMCC;
            }
            else if (scene == "PlayScene")
            {
                PSCC = GameObject.Find("MainCamera").GetComponent<PlaySceneCamController>();
                script.Globals["PSCC"] = PSCC;
            }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void RunLuaScript(string sdirectory,string scene)
        {
            try { 
            Script script = new Script();
            SetLuaGlobalsAndTypes(script, scene);
            script.DoString(File.ReadAllText(sdirectory));
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void CastLuaSpell(string sdirectory, string scene,GameObject target,GameObject caster)
        {
            try {
            Script script = new Script();
            SetLuaGlobalsAndTypes(script, scene);
            script.Call(script.Globals["CastSpell"], target, caster);
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        /////////////////////////////////////
        ///Lua methods from here down
        /////////////////////////////////////
        ///Every structur will need one untell a better way is found
        /////////////////////////////////////


        public Color NewColor(int r, int g, int b)
        {
            try { 
            Color c = new Color(r, g, b);
            return c;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }
        
        /// <summary>
        /// Returns a vector with the suppled floats
        /// </summary>
        /// <param name="x">x coordinates</param>
        /// <param name="y">y coordinates</param>
        /// <returns></returns>
        public Vector2 NewVector2(float x, float y)
        {
            try { 
            Vector2 v = new Vector2(x, y);
            return v;
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ChangeSpriteColor(GameObject G, float r, float g, float b)
        {
            try { 
            G.GetComponent<SpriteRenderer>().color = new Color(r, g, b);
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public void ChangeChildSpriteColor(GameObject G,int ChildNum, float r, float g, float b)
        {
            try { 
            G.transform.GetChild(ChildNum).GetComponent<SpriteRenderer>().color = new Color(r, g, b);
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

        public bool VectorCompair(Vector2 v1, Vector2 v2)
        {
            try { 
            if (v1.x == v2.x && v1.y == v2.y)
            {
                return true;
            }
            else
            {
                return false;
            }
            }
            catch (Exception e)
            {
                GCS.LogController(e.ToString());
                throw;
            }
        }

    }

}