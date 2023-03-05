//#define SUSSYDEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SussySouvScript : MonoBehaviour
{
    [SerializeField]
    private TextMesh _mainText;

    [SerializeField]
    private Transform _answers;

#if SUSSYDEBUG
    public static bool AskQuestions
    {
        private get
        {
            return true;
        }
        set
        {

        }
    }
#else
    public static bool AskQuestions { private get; set; }
#endif
    public static string TDMAnswer { private get; set; }
    public static string TDMQuestion { private get; set; }
    public static string FMAnswer { private get; set; }
    public static string FMQuestion { private get; set; }
    public static string SSQuestion { private get; set; }
    public static string SSAnswer { private get; set; }
    private const string _symbols = "ghidefabcpqrmnojklyz.vwxstu";
    private int _correctAnswer, _id = ++_idc;
    private static int _idc;

    private void Start()
    {
        _mainText.text = "Totally\nnormal.";
        _answers.gameObject.SetActive(false);

        GetComponent<KMBombModule>().OnActivate += () => { StartCoroutine(Activate()); };
        for(int i = 0; i < 6; ++i)
        {
            int j = i;
            _answers.GetChild(i).GetComponent<KMSelectable>().OnInteract += () => { Answer(j); return false; };
        }
    }

    private IEnumerator Activate()
    {
        yield return new WaitUntil(() => AskQuestions);

        _mainText.text = "Where was\nthe number\n" + TDMQuestion + "?";
        List<char> answers = _symbols.Except(TDMAnswer).OrderBy(_ => Random.value).Take(5).Union(TDMAnswer).OrderBy(_ => Random.value).ToList();
        _correctAnswer = answers.IndexOf(TDMAnswer[0]);
        for(int i = 0; i < 6; ++i)
            _answers.GetChild(i).GetComponent<TextMesh>().text = answers[i].ToString();
        _answers.gameObject.SetActive(true);
#if SUSSYDEBUG
        _answers.GetChild(_correctAnswer).GetComponent<TextMesh>().color = Color.red;
#endif

        GetComponent<KMAudio>().PlaySoundAtTransform("Question", transform);
        Debug.Log("[Suspicious Souvenir #" + _id + @"] Asking question: ""Where was the number " + TDMQuestion + @"?""");

        yield return new WaitUntil(() => _correctAnswer == -1);
        for(int i = 0; i < 6; ++i)
        {
#if SUSSYDEBUG
            _answers.GetChild(i).GetComponent<TextMesh>().color = new Color32(255, 248, 221, 255);
#endif
            _answers.GetChild(i).GetComponent<TextMesh>().font = _mainText.font;
            _answers.GetChild(i).GetComponent<Renderer>().material = _mainText.GetComponent<Renderer>().material;
        }
        yield return new WaitForSeconds(Random.Range(.8f, 1.5f));

        _mainText.text = "What was the\nresponse for the\n" + FMQuestion + " stage in Fast\nMath?";
        List<string> numanswers = Enumerable.Range(0, 100).Select(i => i.ToString()).Except(new string[] { FMAnswer }).OrderBy(_ => Random.value).Take(5).Concat(new string[] { FMAnswer }).OrderBy(_ => Random.value).ToList();
        _correctAnswer = numanswers.IndexOf(FMAnswer);
        for(int i = 0; i < 6; ++i)
            _answers.GetChild(i).GetComponent<TextMesh>().text = numanswers[i];
        _answers.gameObject.SetActive(true);

        GetComponent<KMAudio>().PlaySoundAtTransform("Question", transform);
        Debug.Log("[Suspicious Souvenir #" + _id + @"] Asking question: ""What was the response for the " + FMQuestion + @" stage in Fast Math?""");

#if SUSSYDEBUG
        _answers.GetChild(_correctAnswer).GetComponent<TextMesh>().color = Color.red;
#endif
        yield return new WaitUntil(() => _correctAnswer == -1);
#if SUSSYDEBUG
        for(int i = 0; i < 6; ++i)
            _answers.GetChild(i).GetComponent<TextMesh>().color = new Color32(255, 248, 221, 255);
#endif
        yield return new WaitForSeconds(Random.Range(.8f, 1.5f));

        _mainText.text = "What was\ntransmitted for\n" + SSQuestion + "?";
        List<string> stringAnswers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Select(c => c.ToString()).Except(new string[] { SSAnswer }).OrderBy(_ => Random.value).Take(5).Concat(new string[] { SSAnswer }).OrderBy(_ => Random.value).ToList();
        _correctAnswer = stringAnswers.IndexOf(SSAnswer);
        for(int i = 0; i < 6; ++i)
            _answers.GetChild(i).GetComponent<TextMesh>().text = stringAnswers[i];
        _answers.gameObject.SetActive(true);

        GetComponent<KMAudio>().PlaySoundAtTransform("Question", transform);
        Debug.Log("[Suspicious Souvenir #" + _id + @"] Asking question: ""What was transmitted for " + SSQuestion + @"?""");

#if SUSSYDEBUG
        _answers.GetChild(_correctAnswer).GetComponent<TextMesh>().color = Color.red;
#endif
        yield return new WaitUntil(() => _correctAnswer == -1);
        yield return new WaitForSeconds(Random.Range(.8f, 1.5f));

        Debug.Log("[Suspicious Souvenir #" + _id + "] Module (and mission) solved.");
        GetComponent<KMBombModule>().HandlePass();
    }

    private void Answer(int ix)
    {
        if(ix != _correctAnswer)
        {
            Debug.Log("[Suspicious Souvenir #" + _id + "] Wrong answer.");
            GetComponent<KMBombModule>().HandleStrike();
            return;
        }

        GetComponent<KMAudio>().PlaySoundAtTransform("Answer", transform);
        Debug.Log("[Suspicious Souvenir #" + _id + "] Correct answer.");
        _correctAnswer = -1;
        _mainText.text = "";
        _answers.gameObject.SetActive(false);
    }
}
