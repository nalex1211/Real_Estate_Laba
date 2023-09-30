using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Selenium_Test;

[TestClass]
public class LoginTests
{
    private IWebDriver _driver;

    [TestInitialize]
    public void SetUp()
    {
        _driver = new ChromeDriver();
    }

    [TestMethod]
    public void UserCanLogin()
    {
        _driver.Navigate().GoToUrl("https://localhost:44338/Identity/Account/Login");

        var emailField = _driver.FindElement(By.Id("Input_Email"));
        var passwordField = _driver.FindElement(By.Id("Input_Password"));
        var loginButton = _driver.FindElement(By.Id("login-submit"));

        emailField.SendKeys("123@gmail.com");
        passwordField.SendKeys("Bendchat1");
        loginButton.Click();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        Assert.AreEqual("https://localhost:44338/", _driver.Url);
    }

    [TestCleanup]
    public void TearDown()
    {
        _driver.Quit();
    }
}