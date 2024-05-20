using UnityEngine;
using UnityEngine.UI;
using System.Net;
using TMPro;

public class NetworkScreen : MonoBehaviourSingleton<NetworkScreen>
{
    public Button connectBtn;
    public Button startServerBtn;
    public InputField nameInputField;
    public InputField portInputField;
    public InputField addressInputField;

    [Header("Config")]
    [SerializeField] GameObject panelError;
    [SerializeField] TextMeshProUGUI errorText;

    [SerializeField] GameObject winPanel;
    [SerializeField] TextMeshProUGUI winText;

    /// <summary>
    /// Initializes the NetworkScreen by setting up button listeners.
    /// </summary>
    protected override void Initialize()
    {
        connectBtn.onClick.AddListener(OnConnectButtonnClick);
        startServerBtn.onClick.AddListener(OnStartServerButtonClick);
    }

    /// <summary>
    /// Handles the connect button click event. Connects the client to the specified server.
    /// </summary>
    private void OnConnectButtonnClick()
    {
        // Parse the IP address and port from the input fields
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);

        // Start the client with the specified IP, port, and username
        NetworkManager.Instance.StartClient(ipAddress, port, nameInputField.text);

        SwitchToChatScreen();
    }

    /// <summary>
    /// Handles the start server button click event. Starts the server with the specified port.
    /// </summary>
    private void OnStartServerButtonClick()
    {
        // Parse the port from the input field
        int port = System.Convert.ToInt32(portInputField.text);
        NetworkManager.Instance.StartServer(port);
        SwitchToChatScreen();
    }

    /// <summary>
    /// Switches to the chat screen and hides the network screen.
    /// </summary>
    private void SwitchToChatScreen()
    {
        ChatScreen.Instance.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Switches to the menu screen and hides the chat screen.
    /// </summary>
    public void SwitchToMenuScreen()
    {
        GameManager.Instance.timer.text = "";
        ChatScreen.Instance.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the error panel with the specified error message.
    /// </summary>
    /// <param name="errorString">The error message to display.</param>
    public void ShowErrorPanel(string errorString)
    {
        panelError.SetActive(true);
        errorText.text = errorString;

        Invoke(nameof(TurnOffErrorPanel), 3.0f);
    }

    /// <summary>
    /// Turns off the error panel.
    /// </summary>
    private void TurnOffErrorPanel()
    {
        panelError.SetActive(false);
    }

    /// <summary>
    /// Shows the win panel with the specified winner message.
    /// </summary>
    /// <param name="winnerText">The winner message to display.</param>
    public void ShowWinPanel(string winnerText)
    {
        winPanel.SetActive(true);

        winText.text = winnerText;
        Invoke(nameof(TurnOffWinPanel), 5.0f);
    }

    /// <summary>
    /// Turns off the win panel.
    /// </summary>
    private void TurnOffWinPanel()
    {
        winPanel.SetActive(false);
    }
}