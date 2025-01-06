using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class QuizManager : MonoBehaviour
{
    // Canvas Referanslarý
    //public Canvas gameMenuCanvas; // Oyun menüsü Canvas
    //public Canvas resultMenuCanvas; // Sonuç menüsü Canvas

    // Oyun menüsü elemanlarý
    public GameObject gameMenu;  // Oyun menüsü paneli
    public TextMeshProUGUI questionText;  // Soru yazýsý
    public TextMeshProUGUI pointText;  // Puan yazýsý
    public TextMeshProUGUI feedbackText;  // Geri bildirim yazýsý
    public Button optionButton1;  // Seçenek butonlarý
    public Button optionButton2;
    public Button optionButton3;
    public Button optionButton4;
    public Button nextButton;  // Sonraki buton

    // Sonuç menüsü UI Elemanlarý
    public GameObject resultMenu;  // Sonuç menüsü paneli
    public TextMeshProUGUI correctAnswersText;  // Doðru cevap sayýsý
    public TextMeshProUGUI totalPointsText;  // Toplam puan
    public TextMeshProUGUI timeTakenText;  // Geçen süre
    public Button restartButton;  // Yeniden baþlat butonu

    private QuestionList questionList;  // Sorularý tutan liste
    private List<Question> questions;  // Sorularý tutacak liste
    private Question currentQuestion;  // Þu anki soru
    private int currentScore = 0;  // Mevcut puan
    private int totalCorrectAnswers = 0;  // Doðru cevap sayýsý
    private float startTime;  // Baþlangýç zamaný

    private string fileName = "quizQuestions.json";  // JSON dosya adý

    void Start()
    {
        StartCoroutine(LoadQuestionsFromJSON());
        //ActivateCanvas(gameMenuCanvas); // Oyun menüsünü aktif et
        feedbackText.gameObject.SetActive(false); // Geri bildirim gizle
        nextButton.gameObject.SetActive(false); // Sonraki buton gizle
        startTime = Time.time; // Oyun baþlangýç zamaný
        resultMenu.SetActive(false);  // Sonuç menüsünü gizle
    }

    IEnumerator LoadQuestionsFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "quizQuestions.json");

#if UNITY_ANDROID

        filePath = "file://" + filePath;
#endif

        if (File.Exists(filePath))
        {
            // Dosya mevcutsa JSON verisini oku
            string json = File.ReadAllText(filePath);

            // JSON verisini QuestionList sýnýfýna dönüþtür
            questionList = JsonUtility.FromJson<QuestionList>(json);

            if (questionList != null && questionList.Questions.Length > 0)
            {
                // Sorularý listeye dönüþtür
                questions = new List<Question>(questionList.Questions);
                DisplayRandomQuestion();  // Ýlk soruyu göster
            }
            else
            {
                Debug.LogError("Question lists are empty or not exist JSON : " + filePath);
            }
        }
        else
        {
            // Dosya mevcut deðilse hata mesajý göster
            Debug.LogError("Json doesnot exist: " + filePath);
        }

        yield return null;  // Coroutine bitimi
    }

    void DisplayRandomQuestion()
    {
        if (questions.Count == 0)
        {
            ShowResultMenu();  // Sorular bitince sonuç menüsünü göster
            return;  // Sorular bittiði için fonksiyonu sonlandýrýyoruz
        }

        feedbackText.gameObject.SetActive(false);  // Geri bildirim metnini gizle
        EnableOptionButtons();  // Seçenek butonlarýný aktif et

        int randomIndex = Random.Range(0, questions.Count);  // Rastgele bir soru seç
        currentQuestion = questions[randomIndex];
        questionText.text = currentQuestion.questionText;  // Soru metnini güncelle
        optionButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option1;
        optionButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option2;
        optionButton3.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option3;
        optionButton4.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option4;

        questions.RemoveAt(randomIndex);  // Soruyu listeden çýkar

        nextButton.gameObject.SetActive(false);  // Sonraki butonunu gizle
        // Seçenek butonlarý için event listener ekle
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => CheckAnswer(1));
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => CheckAnswer(2));
        optionButton3.onClick.RemoveAllListeners();
        optionButton3.onClick.AddListener(() => CheckAnswer(3));
        optionButton4.onClick.RemoveAllListeners();
        optionButton4.onClick.AddListener(() => CheckAnswer(4));
    }

    void CheckAnswer(int selectedOption)
    {
        feedbackText.gameObject.SetActive(true);  // Geri bildirimi aktif et

        if (selectedOption == currentQuestion.correctAnswerIndex)
        {
            feedbackText.text = "Correct!";  // Doðru cevap
            feedbackText.color = Color.green;  // Yeþil renkte göster

            currentScore += currentQuestion.pointValue;  // Puaný artýr
            totalCorrectAnswers++;  // Doðru cevap sayýsýný artýr
            pointText.text = currentScore.ToString();  // Puaný güncelle

            nextButton.gameObject.SetActive(true);  // Sonraki butonunu aktif et
            nextButton.onClick.RemoveAllListeners();  // Önceki eventleri kaldýr
            nextButton.onClick.AddListener(DisplayRandomQuestion);  // Sonraki soru için listener ekle

            DisableOptionButtons();  // Seçenek butonlarýný pasif et
        }
        else
        {
            feedbackText.text = "Wrong Answer! Try Again.";  // Yanlýþ cevap
            feedbackText.color = Color.red;  // Kýrmýzý renkte göster
            pointText.text = currentScore + "";
            nextButton.gameObject.SetActive(true);  // Sonraki butonunu aktif et
            nextButton.onClick.RemoveAllListeners();  // Önceki eventleri kaldýr
        }
    }

    void DisableOptionButtons()
    {
        optionButton1.interactable = false;  // Seçenek butonlarýný pasif et
        optionButton2.interactable = false;
        optionButton3.interactable = false;
        optionButton4.interactable = false;
    }

    void EnableOptionButtons()
    {
        optionButton1.interactable = true;  // Seçenek butonlarýný aktif et
        optionButton2.interactable = true;
        optionButton3.interactable = true;
        optionButton4.interactable = true;
    }

    void ShowResultMenu()
    {
        //gameMenuCanvas.gameObject.SetActive(false); // Oyun menüsünü gizle
        //resultMenuCanvas.gameObject.SetActive(true); // Sonuç menüsünü göster

        GameObject panel = GameObject.Find("Panel");  // Panel objesinin adýný kullanarak eriþim
        if (panel != null)
        {
            panel.SetActive(false);  // Paneli aktif et
        }
        else
        {
            Debug.LogError("Panel does not exits!");
        }
        resultMenu.SetActive(true);  // Sonuç menüsünü göster

        float timeTaken = Time.time - startTime;  // Geçen süreyi hesapla

        // Sonuçlarý ekranda göster
        correctAnswersText.text = $"Correct Answers: {totalCorrectAnswers}/{questionList.Questions.Length}";
        totalPointsText.text = $"Total Points: {currentScore}";
        timeTakenText.text = $"Time Taken: {timeTaken:F2} seconds";

        // Yeniden baþlatma butonuna listener ekle
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);
    }

    public void RestartQuiz()
    {
        currentScore = 0;  // Puaný sýfýrla
        totalCorrectAnswers = 0;  // Doðru cevap sayýsýný sýfýrla
        pointText.text = "0";  // Puaný sýfýrla
        feedbackText.gameObject.SetActive(false);  // Geri bildirimi gizle
        //resultMenu.SetActive(false);  // Sonuç menüsünü gizle

        // Sorularý sýfýrla
        questions = new List<Question>(questionList.Questions);
        //ActivateCanvas(gameMenuCanvas);
        gameMenu.SetActive(true);  // Oyun menüsünü göster
        resultMenu.SetActive(false);
        startTime = Time.time;  // Yeni baþlangýç zamaný
        DisplayRandomQuestion();  // Ýlk soruyu göster
    }
    void ActivateCanvas(Canvas canvas)
    {
        //gameMenuCanvas.gameObject.SetActive(false);
        //resultMenuCanvas.gameObject.SetActive(false);

        canvas.gameObject.SetActive(true);
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;  // Soru metni
        public string option1;  // Seçenek 1
        public string option2;  // Seçenek 2
        public string option3;  // Seçenek 3
        public string option4;  // Seçenek 4
        public int correctAnswerIndex;  // Doðru cevap indeksi
        public int pointValue;  // Puan deðeri
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] Questions;  // Sorular dizisi
    }
}

