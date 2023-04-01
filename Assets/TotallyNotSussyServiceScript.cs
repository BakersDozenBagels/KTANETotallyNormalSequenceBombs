//#define SUSSYDEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Random = UnityEngine.Random;
using System.Reflection.Emit;

[RequireComponent(typeof(KMService))]
public class TotallyNotSussyServiceScript : MonoBehaviour
{
    private const string VersionString = "1.0";

    [SerializeField]
    private GameObject _prefab;

    private Dictionary<string, object> _module;
    private Coroutine _hook;

    private static bool _tdNoSubmit, _harmed, _harmedTrike, _tbSolved, _isMission, _isTricycle;
    private static Action<int> _tdChange = _ => { }, _ssPress = _ => { };
    private static List<string> _fmStages = new List<string>(), _orgDisplays = new List<string>();
    private static Action<object, string> _mcSet;
    private static int _bossOrg;
    private static List<MonoBehaviour> _orgs = new List<MonoBehaviour>();
    private static List<string> _solvedModules = new List<string>();
    private static List<int[]> _laundrySolutions = new List<int[]>();

    private static int Verify(object settings)
    {
        IList pools = (IList)settings.GetType().GetField("ComponentPools", BindingFlags.Public | BindingFlags.Instance).GetValue(settings);
        List<string> pool = (List<string>)pools[0].GetType().GetField("ModTypes", BindingFlags.Public | BindingFlags.Instance).GetValue(pools[0]);
        if(KTMissionGetter.Mission.ID == "mod_totallyNormalSequenceBombs_sequenceBombs" && pool.Count >= 1 && pool[0] == "SouvenirModule")
            return 1;
        return 0;
    }

    private void Start()
    {
        _module = new Dictionary<string, object>()
        {
            { "Prefab", _prefab },
            { "Condition", new Func<object, int>(Verify) }
        };
        StartCoroutine(DoHook());

        GetComponent<KMGameInfo>().OnStateChange += StateChange;
    }

    private void StateChange(KMGameInfo.State state)
    {
        ResetState();
        if(state == KMGameInfo.State.Gameplay)
            _hook = StartCoroutine(CheckAndHookBomb());

#if SUSSYDEBUG
        if(state == KMGameInfo.State.Setup)
            GetComponent<KMGameCommands>().StartMission("mod_totallyNormalSequenceBombs_tricycle", Random.Range(0, int.MaxValue).ToString());
#endif
    }

    private void ResetState()
    {
        if(_hook != null)
            StopCoroutine(_hook);

        _isMission = false;
        _isTricycle = false;
        _tdNoSubmit = false;
        _tdChange = _ => { };
        _ssPress = _ => { };
        _tbSolved = false;
        _fmStages = new List<string>();
        _orgs.Clear();
        _solvedModules.Clear();
        _laundrySolutions.Clear();
        SussySouvScript.AskQuestions = false;
    }

    private IEnumerator HookTricycle()
    {
        Debug.Log("[Tricycle: Unhinged: Rehinged: Juxtahinged] Version " + VersionString);
        _isTricycle = true;

        //Harmony.DEBUG = true;
        Harmony harm = new Harmony("Tricycle.Unhinged.Rehinged.Juxtahinged");
        Type t;
        MethodInfo mi;

        if(!_harmedTrike)
        {
            do
            {
                t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("TheTwinScript", false)).FirstOrDefault(tp => tp != null);
                if(t == null)
                    yield return null;
            }
            while(t == null);
            t = t.GetNestedType("\u003CTradeInfo\u003Ec__Iterator2", BindingFlags.NonPublic);

            mi = t.GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
            harm.Patch(mi, transpiler: new HarmonyMethod(typeof(TotallyNotSussyServiceScript).GetMethod("TwinTranspiler", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        //do
        //{
        //    t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("Laundry", false)).FirstOrDefault(tp => tp != null);
        //    if(t == null)
        //        yield return null;
        //}
        //while(t == null);

        //mi = t.GetMethod("UseCoin", BindingFlags.NonPublic | BindingFlags.Instance);
        //harm.Patch(mi, prefix: new HarmonyMethod(GetType().GetMethod("LaundryPrefix", BindingFlags.Static | BindingFlags.NonPublic)));

        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("DividedSquaresModule", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);

        MonoBehaviour[] ds;
        do
        {
            ds = FindObjectsOfType(t).Cast<MonoBehaviour>().Where(d => d.gameObject.activeInHierarchy).ToArray();
            yield return null;
        }
        while(ds.Length < 3);
        mi = t.GetMethod("UpdateSolved", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(MonoBehaviour d in ds)
            StartCoroutine((IEnumerator)mi.Invoke(d, new object[] { 3, 0 }));

        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("OrganizationScript", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);

        if(!_harmedTrike)
        {
            mi = t.GetMethod("FixedUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
            harm.Patch(mi, prefix: new HarmonyMethod(GetType().GetMethod("OrgPrefix", BindingFlags.Static | BindingFlags.NonPublic)));
        }

        do
        {
            _orgs = FindObjectsOfType(t).Cast<MonoBehaviour>().Where(d => d.gameObject.activeInHierarchy).ToList();
            yield return null;
        }
        while(_orgs.Count < 3);
        MonoBehaviour bossorg = _orgs.First();
        _bossOrg = (int)t.GetField("moduleId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bossorg);

        yield return null;
        yield return null;

        List<string> mods = new List<string>() {
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Black Hole", "Black Hole",
            "White Hole", "White Hole",
            "Masyu", "UraMasyu",
            "Masyu", "UraMasyu",
            "Clipping Triangles",
            "Flipping Triangles",
            "Slipping Triangles",
            "Tipping Triangles"
        };

        _orgDisplays = mods.Distinct().ToList();
        _orgDisplays = _orgDisplays.OrderBy(_ => Random.value).Take(3).ToList();
        for(int i = 0; i < _orgs.Count; i++)
        {
            //Debug.Log("<><><>");
            //Debug.Log(t);
            //Debug.Log(t.GetField("module", BindingFlags.Public | BindingFlags.Instance));
            //Debug.Log(_orgs[i]);
            //Debug.Log(_orgs[i].GetComponent(t));
            //Debug.Log(t.GetField("module", BindingFlags.Public | BindingFlags.Instance).GetValue(_orgs[i].GetComponent(t)));
            //Debug.Log(((GameObject)t.GetField("module", BindingFlags.Public | BindingFlags.Instance).GetValue(_orgs[i].GetComponent(t))).GetComponent<UnityEngine.UI.Text>());
            ((GameObject)t.GetField("module", BindingFlags.Public | BindingFlags.Instance).GetValue(_orgs[i].GetComponent(t))).GetComponent<UnityEngine.UI.Text>().text = _orgDisplays[i];
        }
    }

    //private static void LaundryPrefix(
    //    Enum[][] ___solutions,
    //    KMBombInfo ___bombInfo,
    //    KMBombModule ___bombModule,
    //    int ___leftKnobPos,
    //    int ___rightKnobPos,
    //    int ___ironingTextPos,
    //    int ___specialTextPos
    //    )
    //{
    //    if(!_isTricycle)
    //        return;

    //    int solves = ___bombInfo.GetSolvedModuleNames().Count % 6;
    //    Enum[] solution = ___solutions[solves];
    //    if(solution.Length == 1)
    //        return; // Unicorn

    //    if(_laundrySolutions.Any(s => s.SequenceEqual(new int[] { ___leftKnobPos, ___rightKnobPos, ___ironingTextPos, ___specialTextPos })))
    //    {
    //        ___bombModule.HandleStrike();
    //        return;
    //    }

    //    if(solution.Cast<int>().SequenceEqual(new int[] { ___leftKnobPos, ___rightKnobPos, ___ironingTextPos, ___specialTextPos }))
    //        _laundrySolutions.Add(new int[] { ___leftKnobPos, ___rightKnobPos, ___ironingTextPos, ___specialTextPos });
    //}

    private static bool OrgPrefix(KMBombInfo ___bomb, ref int ___ticker, int ___moduleId)
    {
        if(!_isTricycle)
            return true;

        if(___moduleId != _bossOrg)
            return false;

        if(++___ticker != 20)
            return false;
        ___ticker = 0;

        int progress = ___bomb.GetSolvedModuleNames().Count;

        List<string> temp = ___bomb.GetSolvedModuleNames();
        foreach(string s in _solvedModules)
            temp.Remove(s);
        _solvedModules = ___bomb.GetSolvedModuleNames();
        if(temp.Count == 0)
            return false;
        string name = temp[0];
        List<string> ignored = new List<string>() { "Organization", "The Twin", "Black", "White", "Divided Squares", "Supermassive Black Hole" };
        if(!_orgDisplays.Contains(name) && !ignored.Contains(name))
            _orgs[0].GetComponent<KMBombModule>().HandleStrike();

        List<string> mods = new List<string>() {
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Metamem", "Macro Memory", "Netherite", "The Stare", "Laundry", "Warning Signs", "Big Circle",
            "Black Hole", "Black Hole",
            "White Hole", "White Hole",
            "Masyu", "UraMasyu",
            "Masyu", "UraMasyu",
            "Clipping Triangles",
            "Flipping Triangles",
            "Slipping Triangles",
            "Tipping Triangles"
        };
        foreach(string mod in ___bomb.GetSolvedModuleNames())
            mods.Remove(mod);

        _orgDisplays = mods.Distinct().ToList();
        if(_orgDisplays.Count == 0)
        {
            foreach(MonoBehaviour org in _orgs)
                org.GetComponent<KMBombModule>().HandlePass();
            _orgDisplays = Enumerable.Repeat("No Modules :)", 3).ToList();
        }
        while(_orgDisplays.Count < 3)
            _orgDisplays.Add(_orgDisplays.PickRandom());
        _orgDisplays = _orgDisplays.OrderBy(_ => Random.value).Take(3).ToList();
        Type t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("OrganizationScript", false)).FirstOrDefault(tp => tp != null);
        for(int i = 0; i < _orgs.Count; i++)
            ((GameObject)t.GetField("module", BindingFlags.Public | BindingFlags.Instance).GetValue(_orgs[i].GetComponent(t))).GetComponent<UnityEngine.UI.Text>().text = _orgDisplays[i];

        return false;
    }

    private static IEnumerable<CodeInstruction> TwinTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        Type t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("TheTwinScript", false)).FirstOrDefault(tp => tp != null);
        FieldInfo fi = t.GetField("_swapCase", BindingFlags.NonPublic | BindingFlags.Instance);
        ParameterExpression p = Expression.Parameter(typeof(int), "i");
        Expression<Func<int, int>> e = Expression.Lambda<Func<int, int>>(
            Expression.Call(typeof(TotallyNotSussyServiceScript).GetMethod("IsTricycle", BindingFlags.NonPublic | BindingFlags.Static), p),
            p
            );
        foreach(CodeInstruction i in instructions)
        {
            if(i.StoresField(fi))
                yield return CodeInstruction.Call(e);
            yield return i;
        }
    }

    private static int IsTricycle(int i)
    {
        return _isTricycle ? 0 : i;
    }

    private IEnumerator CheckAndHookBomb()
    {
        yield return null;
        if(KTMissionGetter.Mission.ID == "mod_totallyNormalSequenceBombs_tricycle")
            StartCoroutine(HookTricycle());
        if(KTMissionGetter.Mission.ID != "mod_totallyNormalSequenceBombs_sequenceBombs")
            yield break;
        _isMission = true;

        StartCoroutine(EnsureHarmony());

        Coroutine cr1 = StartCoroutine(ModifyTDTunnels());
        Coroutine cr2 = StartCoroutine(ModifyPolyMaze());

        Coroutine cr3 = StartCoroutine(ModifyFastMath());
        Coroutine cr4 = StartCoroutine(ModifyTwoBits());

        Coroutine cr6 = StartCoroutine(ModifyMorse());
        Coroutine cr5 = StartCoroutine(ModifySimonSends());

        yield return cr1;
        yield return cr2;
        yield return cr3;
        yield return cr4;
        yield return cr5;
        yield return cr6;

        SussySouvScript.AskQuestions = true;
    }

    private IEnumerator ModifyMorse()
    {
        MonoBehaviour mc;
        do
        {
            mc = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "MorseCodeComponent") as MonoBehaviour;
            if(mc == null)
                yield return null;
        }
        while(mc == null);

        Func<bool> mcSolved = Expression.Lambda<Func<bool>>(
            Expression.Field(
                Expression.Constant(mc),
                mc.GetType().GetField("IsSolved", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            )).Compile();

        while(!mcSolved())
            yield return null;

        Coroutine r = null;
        Action play = () =>
        {
            if(r != null)
                StopCoroutine(r);
            r = StartCoroutine(mc.GetType().GetMethod("LoopRoutine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(mc, new object[0]) as IEnumerator);
        };

        if(_mcSet == null)
        {
            FieldInfo fi = mc.GetType().GetField("chosenWord", BindingFlags.NonPublic | BindingFlags.Instance);

            _mcSet = (o, s) => { fi.SetValue(o, s); };
        }

        int choice = Random.Range(0, 8);
        string[] letters = "abcdefghijklmnopqrstuvwxyz".OrderBy(_ => Random.value).Take(8).Select(c => c.ToString()).ToArray();

        SussySouvScript.SSQuestion = new string[] { "black", "blue", "green", "cyan", "red", "magenta", "yellow", "white" }[choice];
        SussySouvScript.SSAnswer = letters[choice];

        _ssPress = i =>
        {
            _mcSet(mc, letters[i]);
            play();
        };

        MonoBehaviour ss;
        do
        {
            ss = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "SimonSendsModule") as MonoBehaviour;
            if(ss == null)
                yield return null;
        }
        while(ss == null);
        KMSelectable[] buttons = (KMSelectable[])ss.GetType().GetField("Buttons", BindingFlags.Public | BindingFlags.Instance).GetValue(ss);

        for(int i = 0; i < buttons.Length; ++i)
        {
            int j = i;
            buttons[j].OnInteract += () => { SSPress(j); return false; };
        }
    }

    private IEnumerator ModifySimonSends()
    {
        MonoBehaviour ss;
        do
        {
            ss = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "SimonSendsModule") as MonoBehaviour;
            if(ss == null)
                yield return null;
        }
        while(ss == null);

        Func<bool> ssSolved = Expression.Lambda<Func<bool>>(
            Expression.Equal(
                Expression.Field(
                    Expression.Constant(ss),
                    ss.GetType().GetField("_answerSoFar", BindingFlags.NonPublic | BindingFlags.Instance)
                    ),
                Expression.Constant(null, typeof(List<int>))
                )
            ).Compile();

        while(!ssSolved())
            yield return null;
    }

    private void SSPress(int j)
    {
        _ssPress(j);
    }

    private IEnumerator ModifyTwoBits()
    {
        MonoBehaviour tb;
        do
        {
            tb = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "TwoBitsModule") as MonoBehaviour;
            if(tb == null)
                yield return null;
        }
        while(tb == null);

        Func<bool> tbsolved = Expression.Lambda<Func<bool>>(
            Expression.Equal(
                Expression.Convert(
                    Expression.Field(
                        Expression.Constant(tb),
                        tb.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance)
                    ),
                    typeof(int)),
                Expression.Constant(7)
            )).Compile();

        while(!tbsolved())
            yield return null;

        object en = tb.GetType().GetNestedType("State", BindingFlags.NonPublic).GetField("Idle", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        tb.GetType().GetField("currentState", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tb, en);
        _tbSolved = true;
    }

    private IEnumerator ModifyFastMath()
    {
        MonoBehaviour fm;
        do
        {
            fm = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "FastMathModule") as MonoBehaviour;
            if(fm == null)
                yield return null;
        }
        while(fm == null);

        Func<bool> fmsolved = Expression.Lambda<Func<bool>>(
            Expression.Field(
                Expression.Constant(fm),
                fm.GetType().GetField("_isSolved", BindingFlags.NonPublic | BindingFlags.Instance)
            )).Compile();

        while(!fmsolved())
            yield return null;

        CalcTwoBits();
    }

    private void CalcTwoBits()
    {
        MonoBehaviour tb = FindObjectsOfType(typeof(MonoBehaviour)).First(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "TwoBitsModule") as MonoBehaviour;

        Dictionary<string, int> qrs = (Dictionary<string, int>)tb.GetType().GetField("queryResponses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(tb);
        int stage = Random.Range(0, _fmStages.Count);
        SussySouvScript.FMQuestion = new string[] { "first", "second", "third", "fourth", "fifth" }[stage];
        SussySouvScript.FMAnswer = qrs[(_fmStages[stage][0] + _fmStages[stage][2].ToString()).ToLowerInvariant()].ToString();
    }

    private IEnumerator ModifyTDTunnels()
    {
        MonoBehaviour tdt;
        do
        {
            tdt = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "ThreeDTunnels") as MonoBehaviour;
            if(tdt == null)
                yield return null;
        }
        while(tdt == null);

        Func<bool> tdsolved = Expression.Lambda<Func<bool>>(
            Expression.Field(
                Expression.Constant(tdt),
                tdt.GetType().GetField("_solved", BindingFlags.NonPublic | BindingFlags.Instance)
            )).Compile();

        while(!tdsolved())
            yield return null;

        _tdNoSubmit = true;
        tdt.GetType().GetField("_solved", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tdt, false);
    }

    private IEnumerator ModifyPolyMaze()
    {
        MonoBehaviour pm;
        do
        {
            pm = FindObjectsOfType(typeof(MonoBehaviour)).FirstOrDefault(m => ((MonoBehaviour)m).isActiveAndEnabled && m.GetType().Name == "PolyhedralMazeModule") as MonoBehaviour;
            if(pm == null)
                yield return null;
        }
        while(pm == null);

        Func<bool> pmsolved = Expression.Lambda<Func<bool>>(
            Expression.Field(
                Expression.Constant(pm),
                pm.GetType().GetField("_isSolved", BindingFlags.NonPublic | BindingFlags.Instance)
            )).Compile();

        while(!pmsolved())
            yield return null;

        List<int> _numbers = Enumerable.Range(62, 27).ToList();

        _numbers.Shuffle();
        int q = Random.Range(62, 89);

        SussySouvScript.TDMAnswer = "ghidefabcpqrmnojklyz.vwxstu"[_numbers.IndexOf(q)].ToString();
        SussySouvScript.TDMQuestion = q.ToString();

        Transform tens = pm.transform.GetChild(4).GetChild(2);
        Transform ones = pm.transform.GetChild(4).GetChild(3);

        _tdChange = i =>
        {
            SetPolySSD(tens, _numbers[i] / 10);
            SetPolySSD(ones, _numbers[i] % 10);
        };
    }

    private static readonly Dictionary<int, bool[]> _ssDigits = "0#####-#|1#--#---|2-###-##|3#-##-##|4#--###-|5#-#-###|6###-###|7#--#--#|8#######|9#-#####".Split('|').ToDictionary(s => s[0] - '0', s => s.Skip(1).Select(c => c == '#').ToArray());
    private static void SetPolySSD(Transform digit, int x)
    {
        for(int i = 0; i < 7; ++i)
            digit.GetChild(i).gameObject.SetActive(_ssDigits[x][i]);
    }

    private static IEnumerator EnsureHarmony()
    {
        Debug.Log("[Totally Normal Sequence Bombs] Version " + VersionString);
        if(_harmed)
            yield break;

        _harmed = true;

        Harmony h = new Harmony("theseSequenceBombsAreNonSuspicious");

        //Harmony.DEBUG = true;
        Type t;
        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("ThreeDTunnels", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);
        MethodInfo m = t.GetMethod("PressTargetButton", BindingFlags.NonPublic | BindingFlags.Instance);
        MethodInfo p = typeof(TotallyNotSussyServiceScript).GetMethod("TDTPrefix", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, prefix: new HarmonyMethod(p));

        m = t.GetMethod("UpdateDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
        p = typeof(TotallyNotSussyServiceScript).GetMethod("TDTChange", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, postfix: new HarmonyMethod(p));

        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("FastMathModule", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);
        m = t.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        p = typeof(TotallyNotSussyServiceScript).GetMethod("FMReset", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, postfix: new HarmonyMethod(p));

        m = t.GetMethod("GenerateStage", BindingFlags.NonPublic | BindingFlags.Instance);
        p = typeof(TotallyNotSussyServiceScript).GetMethod("FMStageModify", BindingFlags.NonPublic | BindingFlags.Static);
        MethodInfo p2 = typeof(TotallyNotSussyServiceScript).GetMethod("FMStage", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, postfix: new HarmonyMethod(p2), transpiler: new HarmonyMethod(p));

        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("TwoBitsModule", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);
        m = t.GetMethod("OnSubmit", BindingFlags.NonPublic | BindingFlags.Instance);
        p = typeof(TotallyNotSussyServiceScript).GetMethod("TBSubmit", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, prefix: new HarmonyMethod(p));


        do
        {
            t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("MorseCodeComponent", false)).FirstOrDefault(tp => tp != null);
            if(t == null)
                yield return null;
        }
        while(t == null);
        m = t.GetNestedType("\u003CLoopRoutine\u003Ec__Iterator0", BindingFlags.NonPublic).GetMethod("MoveNext", BindingFlags.Public | BindingFlags.Instance);
        p = typeof(TotallyNotSussyServiceScript).GetMethod("MCLoop", BindingFlags.NonPublic | BindingFlags.Static);
        h.Patch(m, transpiler: new HarmonyMethod(p));
    }

    private static IEnumerable<CodeInstruction> MCLoop(IEnumerable<CodeInstruction> instructions)
    {
        Type t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("BombComponent", false)).First(tp => tp != null);
        FieldInfo fi = t.GetField("IsSolved", BindingFlags.Public | BindingFlags.Instance);

        MethodInfo mi = typeof(TotallyNotSussyServiceScript).GetMethod("MCShenanigans", BindingFlags.NonPublic | BindingFlags.Static);
        foreach(CodeInstruction inst in instructions)
        {
            yield return inst;
            if(inst.Is(OpCodes.Ldfld, fi))
                yield return new CodeInstruction(OpCodes.Call, mi);
        }
    }

    private static bool MCShenanigans(bool i)
    {
        if(_isMission)
            return false;
        return i;
    }

    private static IEnumerable<CodeInstruction> FMStageModify(IEnumerable<CodeInstruction> instructions)
    {
        Type t = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetType("FastMathModule", false)).First(tp => tp != null);
        FieldInfo fi = t.GetField("letters", BindingFlags.NonPublic | BindingFlags.Instance);

        MethodInfo mi = typeof(TotallyNotSussyServiceScript).GetMethod("FMRandom", BindingFlags.Public | BindingFlags.Static);
        IEnumerator<CodeInstruction> enu = instructions.GetEnumerator();
        while(enu.MoveNext())
        {
            yield return enu.Current;
            if(enu.Current.Is(OpCodes.Ldc_I4_S, 13))
            {
                enu.MoveNext();
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, fi);
                yield return new CodeInstruction(OpCodes.Call, mi);
            }
        }
    }

    public static int FMRandom(int lower, int upper, string letters)
    {
        if(!_isMission)
            return Random.Range(lower, upper);

        if(letters.All(c => !"BCDEGKPTVZ".Contains(c)))
            throw new NotSupportedException("Why are you using rule seed?");

        int res;
        do
            res = Random.Range(lower, upper);
        while(!"BCDEGKPTVZ".Contains(letters[res]));
        return res;
    }

    private static bool TBSubmit()
    {
        return !_tbSolved;
    }

    private static void FMStage(int num, TextMesh ___Screen)
    {
        if(num == 1)
            _fmStages = new List<string>();
        _fmStages.Add(___Screen.text);
    }

    private static void FMReset()
    {
        _fmStages = new List<string>();
    }

    private static bool TDTPrefix()
    {
        return !_tdNoSubmit;
    }

    private static void TDTChange(TextMesh ___Symbol, int ____location)
    {
        if(_tdNoSubmit)
            ___Symbol.gameObject.SetActive(false);
        _tdChange(____location);
    }

    private IEnumerator DoHook()
    {
        do
        {
            yield return new WaitForSeconds(1f);
        }
        while(!ModdedAPI.TrySet("LoadOnCondition", _module));
    }
}