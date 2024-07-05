using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using BCrypt.Net;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Auth : MonoBehaviour
{
    public GameObject loginPanel, registerPanel, MessagePanel;
    public InputField username, password, confirmPassword, usernameRegister, passwordRegister, emailRegister;
    public Text warningText;
    private string connectionString;

    void Start()
    {
        // Config SQL
        string dbPath = Application.streamingAssetsPath + "/Pig.db";
        connectionString = "URI=file:" + dbPath;
    }

    // Mở Login
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    // Mở Register
    public void OpenRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    // Open MessagePanel
    public void OpenMessage()
    {
        MessagePanel.SetActive(true);
    }

    public void CloseMessage()
    {
        MessagePanel.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerPanel.SetActive(false);
    }

    // Login
    public void LoginUser()
    {
        Debug.Log("Attempting to login...");
        
        // Check if fields are empty
        if (string.IsNullOrEmpty(username.text) || string.IsNullOrEmpty(password.text))
        {
            warningText.text = "Vui lòng nhập đầy đủ thông tin bạn nhé!";
            Debug.Log("Empty fields detected.");
            OpenMessage(); 
            return;
        }

        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (var selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT Password FROM Players WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", username.text);

                    using (var reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHashedPassword = reader.GetString(0);
                            if (BCrypt.Net.BCrypt.Verify(password.text, storedHashedPassword))
                            {
                                Debug.Log("Login successful. Loading MainMenu scene...");
                                warningText.text="đăng nhập thành công bạn chờ chút nhé";
                                MessagePanel.SetActive(false);
                                SceneManager.LoadScene("MainMenu");
                            }
                            else
                            {
                                warningText.text = "Sai tên tài khoản hoặc mật khẩu!";
                                Debug.Log("Incorrect username or password.");
                                OpenMessage(); 
                            }
                        }
                        else
                        {
                            warningText.text = "Tài khoản không tồn tại!";
                            Debug.Log("Username not found.");
                            OpenMessage(); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error " + ex.Message);
                warningText.text = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại.";
                OpenMessage(); 
            }
        }
    }

    // Register
    public void RegisterUser()
    {
        if (string.IsNullOrEmpty(usernameRegister.text) || string.IsNullOrEmpty(passwordRegister.text) ||
            string.IsNullOrEmpty(emailRegister.text) || string.IsNullOrEmpty(confirmPassword.text))
        {
            warningText.text = "Bạn nhập thiếu thông tin kìa bạn ơi...";
            OpenMessage(); 
            return;
        }

        if (!IsPasswordValid(passwordRegister.text))
        {
            warningText.text = "Mật khẩu phải chứa 8-16 ký tự bao gồm chữ hoa, chữ thường và số!";
            OpenMessage(); 
            return;
        }

        if (passwordRegister.text != confirmPassword.text)
        {
            warningText.text = "Mật khẩu không giống nhau :((";
            OpenMessage(); 
            return;
        }

        if (!IsValidEmail(emailRegister.text))
        {
            warningText.text = "Bạn vui lòng nhập đúng Email bạn nhé!";
            OpenMessage(); 
            return;
        }

        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (var selectCommand = connection.CreateCommand())
                {
                    selectCommand.CommandText = "SELECT COUNT(*) FROM Players WHERE Username = @Username";
                    selectCommand.Parameters.AddWithValue("@Username", usernameRegister.text);

                    int userCount = Convert.ToInt32(selectCommand.ExecuteScalar());
                    if (userCount > 0)
                    {
                        warningText.text = "Tài khoản đã tồn tại!";
                        OpenMessage(); 
                        return;
                    }
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordRegister.text);
                using (var insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = "INSERT INTO Players (Username, Password, Email) VALUES (@username, @password, @Email)";
                    insertCommand.Parameters.AddWithValue("@username", usernameRegister.text);
                    insertCommand.Parameters.AddWithValue("@password", hashedPassword);
                    insertCommand.Parameters.AddWithValue("@Email", emailRegister.text);
                    insertCommand.ExecuteNonQuery();
                }

                warningText.text = "Đăng ký thành công!";
                OpenMessage(); 
            }
            catch (Exception ex)
            {
                Debug.LogError("Đã xảy ra lỗi khi đăng ký: " + ex.Message);
                warningText.text = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.";
                OpenMessage(); 
            }
        }
    }

    private bool IsPasswordValid(string password)
    {
        // Regular expression for password validation
        string pattern = @"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{8,16}$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(password);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(email);
    }
}
