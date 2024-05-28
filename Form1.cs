using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace xprox_encryptor_decryptor
{
    public partial class Form1 : Form
    {
        string code = "", codeEncrypt = "", codeDecryptor = "";
        private static byte[] fixedKey = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static byte[] fixedIV = Encoding.UTF8.GetBytes("1234567890123456");

        public Form1()
        {
            InitializeComponent();
            
            radioBtn1_encrypt_decryptor.CheckedChanged += RadioButton_CheckedChanged;
            radioBtn2_encrypt_decryptor.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            CheckActiveRadioButton();
        }

        private void CheckActiveRadioButton()
        {
            if (radioBtn1_encrypt_decryptor.Checked)
            {
                if (IsPlainText(code))
                {
                    codeEncrypt = EncryptText(code);
                    richTextBox2_encryptor_decryptor.Text = codeEncrypt;
                    if (string.IsNullOrEmpty(codeEncrypt))
                    {
                        richTextBox2_encryptor_decryptor.Text = "";
                    }
                }
                else
                {
                    richTextBox2_encryptor_decryptor.Text = "";
                }
            }
            else if (radioBtn2_encrypt_decryptor.Checked)
            {
                if (IsBase64String(code))
                {
                    codeDecryptor = DecryptText(codeEncrypt);
                    richTextBox2_encryptor_decryptor.Text = codeDecryptor;
                    if (string.IsNullOrEmpty(codeDecryptor))
                    {
                        richTextBox2_encryptor_decryptor.Text = "";
                    }
                }
                else
                {
                    richTextBox2_encryptor_decryptor.Text = "";
                }
            }
        }

        private void richTextBox1_encrypt_decryptor_TextChanged(object sender, EventArgs e)
        {
            code = richTextBox1_encrypt_decryptor.Text;
        }
        private void richTextBox2_encryptor_decryptor_TextChanged(object sender, EventArgs e)
        {
            
        }

        private string EncryptText(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = fixedKey;
                aesAlg.IV = fixedIV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(code);
                        }
                        byte[] encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
        }

        private string DecryptText(string encrypt)
        {
            if (string.IsNullOrEmpty(encrypt))
                return string.Empty;

            byte[] cipherText = Convert.FromBase64String(encrypt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = fixedKey;
                aesAlg.IV = fixedIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private bool IsPlainText(string input)
        {
            input = input.Trim();
            return Regex.IsMatch(input, @"^[a-zA-Z0-9\s]*$");
        }

        private bool IsBase64String(string input)
        {
            input = input.Trim();
            return (input.Length % 4 == 0) && Regex.IsMatch(input, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

    }
}