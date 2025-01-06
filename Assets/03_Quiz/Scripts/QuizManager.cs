using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class QuizManager : MonoBehaviour
{
    // Canvas Referanslar�
    //public Canvas gameMenuCanvas; // Oyun men�s� Canvas
    //public Canvas resultMenuCanvas; // Sonu� men�s� Canvas

    // Oyun men�s� elemanlar�
    public GameObject gameMenu;  // Oyun men�s� paneli
    public TextMeshProUGUI questionText;  // Soru yaz�s�
    public TextMeshProUGUI pointText;  // Puan yaz�s�
    public TextMeshProUGUI feedbackText;  // Geri bildirim yaz�s�
    public Button optionButton1;  // Se�enek butonlar�
    public Button optionButton2;
    public Button optionButton3;
    public Button optionButton4;
    public Button nextButton;  // Sonraki buton

    // Sonu� men�s� UI Elemanlar�
    public GameObject resultMenu;  // Sonu� men�s� paneli
    public TextMeshProUGUI correctAnswersText;  // Do�ru cevap say�s�
    public TextMeshProUGUI totalPointsText;  // Toplam puan
    public TextMeshProUGUI timeTakenText;  // Ge�en s�re
    public Button restartButton;  // Yeniden ba�lat butonu

    private QuestionList questionList;  // Sorular� tutan liste
    private List<Question> questions;  // Sorular� tutacak liste
    private Question currentQuestion;  // �u anki soru
    private int currentScore = 0;  // Mevcut puan
    private int totalCorrectAnswers = 0;  // Do�ru cevap say�s�
    private float startTime;  // Ba�lang�� zaman�

    private string fileName = "quizQuestions.json";  // JSON dosya ad�

    void Start()
    {
        StartCoroutine(LoadQuestionsFromJSON());
        //ActivateCanvas(gameMenuCanvas); // Oyun men�s�n� aktif et
        feedbackText.gameObject.SetActive(false); // Geri bildirim gizle
        nextButton.gameObject.SetActive(false); // Sonraki buton gizle
        startTime = Time.time; // Oyun ba�lang�� zaman�
        resultMenu.SetActive(false);  // Sonu� men�s�n� gizle
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

            // JSON verisini QuestionList s�n�f�na d�n��t�r
            questionList = JsonUtility.FromJson<QuestionList>(json);

            if (questionList != null && questionList.Questions.Length > 0)
            {
                // Sorular� listeye d�n��t�r
                questions = new List<Question>(questionList.Questions);
                DisplayRandomQuestion();  // �lk soruyu g�ster
            }
            else
            {
                Debug.LogError("Question lists are empty or not exist JSON : " + filePath);
            }
        }
        else
        {
            // Dosya mevcut de�ilse hata mesaj� g�ster
            Debug.LogError("Json doesnot exist: " + filePath);
        }

        yield return null;  // Coroutine bitimi
    }

    void DisplayRandomQuestion()
    {
        if (questions.Count == 0)
        {
            ShowResultMenu();  // Sorular bitince sonu� men�s�n� g�ster
            return;  // Sorular bitti�i i�in fonksiyonu sonland�r�yoruz
        }

        feedbackText.gameObject.SetActive(false);  // Geri bildirim metnini gizle
        EnableOptionButtons();  // Se�enek butonlar�n� aktif et

        int randomIndex = Random.Range(0, questions.Count);  // Rastgele bir soru se�
        currentQuestion = questions[randomIndex];
        questionText.text = currentQuestion.questionText;  // Soru metnini g�ncelle
        optionButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option1;
        optionButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option2;
        optionButton3.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option3;
        optionButton4.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option4;

        questions.RemoveAt(randomIndex);  // Soruyu listeden ��kar

        nextButton.gameObject.SetActive(false);  // Sonraki butonunu gizle
        // Se�enek butonlar� i�in event listener ekle
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
            feedbackText.text = "Correct!";  // Do�ru cevap
            feedbackText.color = Color.green;  // Ye�il renkte g�ster

            currentScore += currentQuestion.pointValue;  // Puan� art�r
            totalCorrectAnswers++;  // Do�ru cevap say�s�n� art�r
            pointText.text = currentScore.ToString();  // Puan� g�ncelle

            nextButton.gameObject.SetActive(true);  // Sonraki butonunu aktif et
            nextButton.onClick.RemoveAllListeners();  // �nceki eventleri kald�r
            nextButton.onClick.AddListener(DisplayRandomQuestion);  // Sonraki soru i�in listener ekle

            DisableOptionButtons();  // Se�enek butonlar�n� pasif et
        }
        else
        {
            feedbackText.text = "Wrong Answer! Try Again.";  // Yanl�� cevap
            feedbackText.color = Color.red;  // K�rm�z� renkte g�ster
            pointText.text = currentScore + "";
            nextButton.gameObject.SetActive(true);  // Sonraki butonunu aktif et
            nextButton.onClick.RemoveAllListeners();  // �nceki eventleri kald�r
        }
    }

    void DisableOptionButtons()
    {
        optionButton1.interactable = false;  // Se�enek butonlar�n� pasif et
        optionButton2.interactable = false;
        optionButton3.interactable = false;
        optionButton4.interactable = false;
    }

    void EnableOptionButtons()
    {
        optionButton1.interactable = true;  // Se�enek butonlar�n� aktif et
        optionButton2.interactable = true;
        optionButton3.interactable = true;
        optionButton4.interactable = true;
    }

    void ShowResultMenu()
    {
        //gameMenuCanvas.gameObject.SetActive(false); // Oyun men�s�n� gizle
        //resultMenuCanvas.gameObject.SetActive(true); // Sonu� men�s�n� g�ster

        GameObject panel = GameObject.Find("Panel");  // Panel objesinin ad�n� kullanarak eri�im
        if (panel != null)
        {
            panel.SetActive(false);  // Paneli aktif et
        }
        else
        {
            Debug.LogError("Panel does not exits!");
        }
        resultMenu.SetActive(true);  // Sonu� men�s�n� g�ster

        float timeTaken = Time.time - startTime;  // Ge�en s�reyi hesapla

        // Sonu�lar� ekranda g�ster
        correctAnswersText.text = $"Correct Answers: {totalCorrectAnswers}/{questionList.Questions.Length}";
        totalPointsText.text = $"Total Points: {currentScore}";
        timeTakenText.text = $"Time Taken: {timeTaken:F2} seconds";

        // Yeniden ba�latma butonuna listener ekle
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartQuiz);
    }

    public void RestartQuiz()
    {
        currentScore = 0;  // Puan� s�f�rla
        totalCorrectAnswers = 0;  // Do�ru cevap say�s�n� s�f�rla
        pointText.text = "0";  // Puan� s�f�rla
        feedbackText.gameObject.SetActive(false);  // Geri bildirimi gizle
        //resultMenu.SetActive(false);  // Sonu� men�s�n� gizle

        // Sorular� s�f�rla
        questions = new List<Question>(questionList.Questions);
        //ActivateCanvas(gameMenuCanvas);
        gameMenu.SetActive(true);  // Oyun men�s�n� g�ster
        resultMenu.SetActive(false);
        startTime = Time.time;  // Yeni ba�lang�� zaman�
        DisplayRandomQuestion();  // �lk soruyu g�ster
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
        public string option1;  // Se�enek 1
        public string option2;  // Se�enek 2
        public string option3;  // Se�enek 3
        public string option4;  // Se�enek 4
        public int correctAnswerIndex;  // Do�ru cevap indeksi
        public int pointValue;  // Puan de�eri
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] Questions;  // Sorular dizisi
    }
}

