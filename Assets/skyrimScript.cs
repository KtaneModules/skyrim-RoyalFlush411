using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using skyrim;

public class skyrimScript : MonoBehaviour
{
    //Bomb info
    public KMBombInfo Bomb;
    public KMAudio Audio;
    KMBombModule BombModule;

    //Buttons
    public KMSelectable race;
    public KMSelectable weapon;
    public KMSelectable enemy;
    public KMSelectable city;
    public KMSelectable shout;
    public KMSelectable cycleUp;
    public KMSelectable accept;
    public KMSelectable cycleDown;
    public KMSelectable submit;

    //Image lists
    public List<Texture> raceImages;
    public List<Texture> weaponImages;
    public List<Texture> enemyImages;
    public List<Texture> cityImages;
    private List<String> shoutOptions = new List<string> { "fus\nro\ndah", "zun\nhal\nvik", "liz\nslen\nnus", "wuld\nnah\nkest", "jor\nzah\nfrul", "fas\nru\nmar", "yol\ntor\nshul", "kan\ndrem\nov", "tid\nklo\nul" };
    private List<String> shoutNameOptions = new List<string> { "Unrelenting Force", "Disarm", "Ice Form", "Whirlwind Sprint", "Dragonrend", "Dismay", "Fire Breath", "Kyne's Peace", "Slow Time" };
    private List<String> allValidNames = new List<string>();

    //Renderers
    public Renderer mainDisplay;
    public Renderer raceRenderer;
    public Renderer weaponRenderer;
    public Renderer enemyRenderer;
    public Renderer cityRenderer;
    public Renderer shoutRenderer;

    //Correct answer lists
    public List<Texture> level1a;
    public List<Texture> level1b;
    public List<Texture> level1c;
    public List<Texture> level2a;
    public List<Texture> level2b;
    public List<Texture> level2c;
    public List<Texture> level2d;
    public List<Texture> level3a;
    public List<Texture> level3b;
    public List<Texture> level3c;
    public List<Texture> level3d;
    public List<Texture> level3e;
    public List<Texture> level4;
    private List<String> level5 = new List<string> { "fus\nro\ndah", "zun\nhal\nvik", "liz\nslen\nnus", "wuld\nnah\nkest", "jor\nzah\nfrul", "fas\nru\nmar", "yol\ntor\nshul", "kan\ndrem\nov", "tid\nklo\nul" };
    private Texture correctRace;
    private Texture correctWeapon;
    private Texture correctEnemy;
    private Texture correctCity;
    private String correctShout;
    private String shoutName;

    //Button Text Meshes
    public TextMesh raceText;
    public TextMesh weaponText;
    public TextMesh enemyText;
    public TextMesh cityText;
    public TextMesh shoutLabel;
    public TextMesh shoutText;
    public TextMesh mainShoutText;

    //Misc textures
    public Texture screenMat;

    //Ints & bools
    int raceIndex;
    int weaponIndex;
    int enemyIndex;
    int cityIndex;
    int shoutIndex;
    bool raceActive = true;
    bool weaponActive = false;
    bool enemyActive = false;
    bool cityActive = false;
    bool shoutActive = false;
    private char firstSerialCharacter;
    bool solved = false;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;

    //Set-up
    void Awake()
    {
        moduleId = moduleIdCounter++;
        race.OnInteract += delegate () { keyPress(race); return false; };
        weapon.OnInteract += delegate () { keyPress(weapon); return false; };
        enemy.OnInteract += delegate () { keyPress(enemy); return false; };
        city.OnInteract += delegate () { keyPress(city); return false; };
        shout.OnInteract += delegate () { keyPress(shout); return false; };
        cycleUp.OnInteract += delegate () { OncycleUp(); return false; };
        accept.OnInteract += delegate () { Onaccept(); return false; };
        cycleDown.OnInteract += delegate () { OncycleDown(); return false; };
        submit.OnInteract += delegate () { Onsubmit(); return false; };
    }

    void Start()
    {
        allValidNames.AddRange(raceImages.Select(t => t.name));
        allValidNames.AddRange(weaponImages.Select(t => t.name));
        allValidNames.AddRange(enemyImages.Select(t => t.name));
        allValidNames.AddRange(cityImages.Select(t => t.name));
        allValidNames.AddRange(shoutNameOptions);

        firstSerialCharacter = Bomb.GetSerialNumber().First();
        racePicker();
        weaponPicker();
        enemyPicker();
        cityPicker();
        shoutPicker();
        shoutText.gameObject.SetActive(false);
        shoutRenderer.gameObject.SetActive(false);
        raceAnswer();
        weaponAnswer();
        enemyAnswer();
        cityAnswer();
        shoutAnswer();
        mainDisplay.material.mainTexture = raceImages[raceIndex];
    }

    void racePicker()
    {
        while (raceImages.Count > 3)
        {
            raceIndex = UnityEngine.Random.Range(0, raceImages.Count);
            raceImages.RemoveAt(raceIndex);
            raceIndex = 0;
        }
        Debug.LogFormat("[Skyrim #{0}] The chosen races are {1}.", moduleId, string.Join(", ", raceImages.Select((x) => x.name).ToArray()));
    }

    void weaponPicker()
    {
        while (weaponImages.Count > 3)
        {
            weaponIndex = UnityEngine.Random.Range(0, weaponImages.Count);
            weaponImages.RemoveAt(weaponIndex);
            weaponIndex = 0;
        }
        Debug.LogFormat("[Skyrim #{0}] The chosen weapons are {1}.", moduleId, string.Join(", ", weaponImages.Select((x) => x.name).ToArray()));
    }

    void enemyPicker()
    {
        while (enemyImages.Count > 3)
        {
            enemyIndex = UnityEngine.Random.Range(0, enemyImages.Count);
            enemyImages.RemoveAt(enemyIndex);
            enemyIndex = 0;
        }
        Debug.LogFormat("[Skyrim #{0}] The chosen enemies are {1}.", moduleId, string.Join(", ", enemyImages.Select((x) => x.name).ToArray()));

    }

    void cityPicker()
    {
        while (cityImages.Count > 3)
        {
            cityIndex = UnityEngine.Random.Range(0, cityImages.Count);
            cityImages.RemoveAt(cityIndex);
            cityIndex = 0;
        }
        cityImages = cityImages.OrderBy(image => level4.IndexOf(image)).ToList();
        Debug.LogFormat("[Skyrim #{0}] The chosen cities are {1}.", moduleId, string.Join(", ", cityImages.Select((x) => x.name).ToArray()));
    }

    void shoutPicker()
    {
        while (shoutOptions.Count > 3)
        {
            shoutIndex = UnityEngine.Random.Range(0, shoutOptions.Count);
            shoutOptions.RemoveAt(shoutIndex);
            shoutNameOptions.RemoveAt(shoutIndex);
            shoutIndex = 0;
        }
        Debug.LogFormat("[Skyrim #{0}] The chosen shouts are {1} ({2}), {3} ({4}), {5} ({6}).", moduleId, shoutOptions[0].Replace("\n", "-"), shoutNameOptions[0], shoutOptions[1].Replace("\n", "-"), shoutNameOptions[1], shoutOptions[2].Replace("\n", "-"), shoutNameOptions[2]);
    }

    void raceAnswer()
    {
        if (raceImages.Any(x => x.name == "Imperial"))
        {
            raceImages = raceImages.OrderBy(image => level1a.IndexOf(image)).ToList();
        }
        else if (raceImages.Any(x => x.name == "Nord"))
        {
            raceImages = raceImages.OrderBy(image => level1b.IndexOf(image)).ToList();
        }
        else
        {
            raceImages = raceImages.OrderBy(image => level1c.IndexOf(image)).ToList();
        }
        correctRace = raceImages[0];
        Debug.LogFormat("[Skyrim #{0}] The correct race is {1}.", moduleId, raceImages[0].name);
        raceIndex = UnityEngine.Random.Range(0, raceImages.Count);
    }

    void weaponAnswer()
    {
        if (raceImages.Any(x => x.name == "Breton") && correctRace.name != "Breton")
        {
            weaponImages = weaponImages.OrderBy(image => level2a.IndexOf(image)).ToList();
        }
        else if (raceImages.Any(x => x.name == "Orc") && correctRace.name != "Orc")
        {
            weaponImages = weaponImages.OrderBy(image => level2b.IndexOf(image)).ToList();
        }
        else if (correctRace.name == "Redguard")
        {
            weaponImages = weaponImages.OrderBy(image => level2c.IndexOf(image)).ToList();
        }
        else
        {
            weaponImages = weaponImages.OrderBy(image => level2d.IndexOf(image)).ToList();
        }
        correctWeapon = weaponImages[0];
        Debug.LogFormat("[Skyrim #{0}] The correct weapon is {1}.", moduleId, weaponImages[0].name);
        weaponIndex = UnityEngine.Random.Range(0, weaponImages.Count);
    }

    void enemyAnswer()
    {
        if (correctWeapon.name == "Bow of the Hunt" || correctWeapon.name == "Firiniel's End")
        {
            enemyImages = enemyImages.OrderBy(image => level3a.IndexOf(image)).ToList();
        }
        else if (correctWeapon.name == "Volendrung" || correctWeapon.name == "Dawnbreaker" || correctWeapon.name == "Mace of Molag Bal")
        {
            enemyImages = enemyImages.OrderBy(image => level3b.IndexOf(image)).ToList();
        }
        else if (correctWeapon.name == "Windshear" || correctWeapon.name == "Chillrend")
        {
            enemyImages = enemyImages.OrderBy(image => level3c.IndexOf(image)).ToList();
        }
        else if (correctWeapon.name == "Axe of Whiterun")
        {
            enemyImages = enemyImages.OrderBy(image => level3d.IndexOf(image)).ToList();
        }
        else
        {
            enemyImages = enemyImages.OrderBy(image => level3e.IndexOf(image)).ToList();
        }
        correctEnemy = enemyImages[0];
        Debug.LogFormat("[Skyrim #{0}] The correct enemy is {1}.", moduleId, enemyImages[0].name);
        enemyIndex = UnityEngine.Random.Range(0, enemyImages.Count);
    }

    Texture cityFinder(int initialOffset)
    {
        for (int index = initialOffset; index < (initialOffset + level4.Count); ++index)
        {
            string cityName = level4[index % level4.Count].name;

            Texture validCityImage = cityImages.FirstOrDefault((x) => x.name == cityName);
            if (validCityImage != null)
            {
                return validCityImage;
            }
        }

        return null;
    }

    void cityAnswer()
    {
        if (correctRace.name == "Nord")
        {
            if (raceImages.Any(x => x.name == "Argonian"))
            {
                correctCity = cityFinder(6);
            }
            else
            {
                correctCity = cityFinder(3);
            }
        }
        else if (correctRace.name == "Khajiit")
        {
            if (enemyImages.Any(x => x.name == "Frost Troll"))
            {
                correctCity = cityFinder(4);
            }
            else
            {
                correctCity = cityFinder(7);
            }
        }
        else if (correctRace.name == "Breton")
        {
            if (correctWeapon.name == "Blade of Woe")
            {
                correctCity = cityFinder(1);
            }
            else
            {
                correctCity = cityFinder(5);
            }
        }
        else if (correctRace.name == "Argonian")
        {
            if (correctEnemy.name == "Draugr Overlord")
            {
                correctCity = cityFinder(8);
            }
            else
            {
                correctCity = cityFinder(1);
            }
        }
        else if (correctRace.name == "Dunmer")
        {
            if (enemyImages.Any(x => x.name == "Mudcrab"))
            {
                correctCity = cityFinder(0);
            }
            else
            {
                correctCity = cityFinder(7);
            }
        }
        else if (correctRace.name == "Altmer")
        {
            if (correctWeapon.name == "Windshear")
            {
                correctCity = cityFinder(5);
            }
            else
            {
                correctCity = cityFinder(0);
            }
        }
        else if (correctRace.name == "Redguard")
        {
            if (!weaponImages.Any(x => x.name == "Dawnbreaker" || x.name == "Volendrung" || x.name == "Mace of Molag Bal"))
            {
                correctCity = cityFinder(3);
            }
            else
            {
                correctCity = cityFinder(8);
            }
        }
        else if (correctRace.name == "Orc")
        {
            if (correctEnemy.name == "Cave Bear")
            {
                correctCity = cityFinder(2);
            }
            else
            {
                correctCity = cityFinder(4);
            }
        }
        else if (correctRace.name == "Imperial")
        {
            if (weaponImages.Any(x => x.name == "Volendrung"))
            {
                correctCity = cityFinder(7);
            }
            else
            {
                correctCity = cityFinder(2);
            }
        }
        Debug.LogFormat("[Skyrim #{0}] The correct home city is {1}.", moduleId, correctCity.name);
    }

    String shoutFinder(int initialOffsetShout)
    {
        for (int index = initialOffsetShout; index < (initialOffsetShout + level5.Count); ++index)
        {
            string shoutName = level5[index % level5.Count];

            String validShout = shoutOptions.FirstOrDefault((x) => x == shoutName);
            if (validShout != null)
            {
                return validShout;
            }
        }

        return null;
    }

    void shoutAnswer()
    {
        if ((int) firstSerialCharacter >= 'A' && (int) firstSerialCharacter <= 'Z')
        {
            if (correctEnemy.name == "Draugr Overlord")
            {
                correctShout = shoutFinder(0);
            }
            else if (correctEnemy.name == "Frost Troll")
            {
                correctShout = shoutFinder(1);
            }
            else if (correctEnemy.name == "Blood Dragon")
            {
                correctShout = shoutFinder(2);
            }
            else if (correctEnemy.name == "Frostbite Spider")
            {
                correctShout = shoutFinder(3);
            }
            else if (correctEnemy.name == "Dragon Priest")
            {
                correctShout = shoutFinder(4);
            }
            else if (correctEnemy.name == "Mudcrab")
            {
                correctShout = shoutFinder(5);
            }
            else if (correctEnemy.name == "Cave Bear")
            {
                correctShout = shoutFinder(6);
            }
            else if (correctEnemy.name == "Draugr")
            {
                correctShout = shoutFinder(7);
            }
            else if (correctEnemy.name == "Alduin")
            {
                correctShout = shoutFinder(8);
            }
        }
        else
        {
            int number = (int) (firstSerialCharacter - '0');
            if (number % 2 == 0)
            {
                if (correctEnemy.name == "Alduin")
                {
                    correctShout = shoutFinder(0);
                }
                else if (correctEnemy.name == "Mudcrab")
                {
                    correctShout = shoutFinder(1);
                }
                else if (correctEnemy.name == "Cave Bear")
                {
                    correctShout = shoutFinder(2);
                }
                else if (correctEnemy.name == "Draugr Overlord")
                {
                    correctShout = shoutFinder(3);
                }
                else if (correctEnemy.name == "Blood Dragon")
                {
                    correctShout = shoutFinder(4);
                }
                else if (correctEnemy.name == "Draugr")
                {
                    correctShout = shoutFinder(5);
                }
                else if (correctEnemy.name == "Frostbite Spider")
                {
                    correctShout = shoutFinder(6);
                }
                else if (correctEnemy.name == "Dragon Priest")
                {
                    correctShout = shoutFinder(7);
                }
                else if (correctEnemy.name == "Frost Troll")
                {
                    correctShout = shoutFinder(8);
                }
            }
            else
            {
                if (correctEnemy.name == "Draugr")
                {
                    correctShout = shoutFinder(0);
                }
                else if (correctEnemy.name == "Dragon Priest")
                {
                    correctShout = shoutFinder(1);
                }
                else if (correctEnemy.name == "Mudcrab")
                {
                    correctShout = shoutFinder(2);
                }
                else if (correctEnemy.name == "Frost Troll")
                {
                    correctShout = shoutFinder(3);
                }
                else if (correctEnemy.name == "Alduin")
                {
                    correctShout = shoutFinder(4);
                }
                else if (correctEnemy.name == "Draugr Overlord")
                {
                    correctShout = shoutFinder(5);
                }
                else if (correctEnemy.name == "Blood Dragon")
                {
                    correctShout = shoutFinder(6);
                }
                else if (correctEnemy.name == "Cave Bear")
                {
                    correctShout = shoutFinder(7);
                }
                else if (correctEnemy.name == "Frostbite Spider")
                {
                    correctShout = shoutFinder(8);
                }
            }
        }
        if (correctShout == "fus\nro\ndah")
        {
            shoutName = "Unrelenting Force";
        }
        else if (correctShout == "zun\nhal\nvik")
        {
            shoutName = "Disarm";
        }
        else if (correctShout == "liz\nslen\nnus")
        {
            shoutName = "Ice Form";
        }
        else if (correctShout == "wuld\nnah\nkest")
        {
            shoutName = "Whirlwind Sprint";
        }
        else if (correctShout == "jor\nzah\nfrul")
        {
            shoutName = "Dragonrend";
        }
        else if (correctShout == "fas\nru\nmar")
        {
            shoutName = "Dismay";
        }
        else if (correctShout == "yol\ntor\nshul")
        {
            shoutName = "Fire Breath";
        }
        else if (correctShout == "kan\ndrem\nov")
        {
            shoutName = "Kyne's Peace";
        }
        else if (correctShout == "tid\nklo\nul")
        {
            shoutName = "Slow Time";
        }
        Debug.LogFormat("[Skyrim #{0}] The correct shout is {1} ({2}).", moduleId, correctShout.Replace("\n", "-"), shoutName);
    }

    //Buttons
    public void keyPress(KMSelectable buttonName)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        cycleDown.AddInteractionPunch(.5f);
        if (buttonName == race)
        {
            shoutRenderer.gameObject.SetActive(false);
            weaponActive = false;
            enemyActive = false;
            cityActive = false;
            shoutActive = false;
            raceActive = true;
            mainDisplay.material.mainTexture = raceImages[raceIndex];
        }
        else if (buttonName == weapon)
        {
            shoutRenderer.gameObject.SetActive(false);
            enemyActive = false;
            cityActive = false;
            raceActive = false;
            shoutActive = false;
            weaponActive = true;
            mainDisplay.material.mainTexture = weaponImages[weaponIndex];
        }
        else if (buttonName == enemy)
        {
            shoutRenderer.gameObject.SetActive(false);
            cityActive = false;
            raceActive = false;
            weaponActive = false;
            shoutActive = false;
            enemyActive = true;
            mainDisplay.material.mainTexture = enemyImages[enemyIndex];
        }
        else if (buttonName == city)
        {
            shoutRenderer.gameObject.SetActive(false);
            raceActive = false;
            weaponActive = false;
            enemyActive = false;
            shoutActive = false;
            cityActive = true;
            mainDisplay.material.mainTexture = cityImages[cityIndex];
        }
        else if (buttonName == shout)
        {
            raceActive = false;
            weaponActive = false;
            enemyActive = false;
            cityActive = false;
            shoutActive = true;
            shoutRenderer.gameObject.SetActive(true);
            mainShoutText.text = shoutOptions[shoutIndex];
            mainDisplay.material.mainTexture = screenMat;
        }
    }

    public void OncycleUp()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        cycleUp.AddInteractionPunch(.5f);
        if (raceActive == true)
        {
            raceIndex = ((raceIndex + raceImages.Count) - 1) % raceImages.Count;
            mainDisplay.material.mainTexture = raceImages[raceIndex];
        }
        else if (weaponActive == true)
        {
            weaponIndex = ((weaponIndex + weaponImages.Count) - 1) % weaponImages.Count;
            mainDisplay.material.mainTexture = weaponImages[weaponIndex];
        }
        else if (enemyActive == true)
        {
            enemyIndex = ((enemyIndex + enemyImages.Count) - 1) % enemyImages.Count;
            mainDisplay.material.mainTexture = enemyImages[enemyIndex];
        }
        else if (cityActive == true)
        {
            cityIndex = ((cityIndex + cityImages.Count) - 1) % cityImages.Count;
            mainDisplay.material.mainTexture = cityImages[cityIndex];
        }
        else if (shoutActive == true)
        {
            shoutIndex = ((shoutIndex + shoutOptions.Count) - 1) % shoutOptions.Count;
            mainDisplay.material.mainTexture = screenMat;
            mainShoutText.text = shoutOptions[shoutIndex];
        }
    }

    public void Onaccept()
    {

        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        cycleUp.AddInteractionPunch(.5f);
        if (raceActive == true)
        {
            raceRenderer.material.mainTexture = mainDisplay.material.mainTexture;
            raceText.text = "";
        }
        else if (weaponActive == true)
        {
            weaponRenderer.material.mainTexture = mainDisplay.material.mainTexture;
            weaponText.text = "";
        }
        else if (enemyActive == true)
        {
            enemyRenderer.material.mainTexture = mainDisplay.material.mainTexture;
            enemyText.text = "";
        }
        else if (cityActive == true)
        {
            cityRenderer.material.mainTexture = mainDisplay.material.mainTexture;
            cityText.text = "";
        }
        else if (shoutActive == true)
        {
            shoutText.gameObject.SetActive(true);
            shoutText.text = mainShoutText.text;
            shoutLabel.text = "";
        }
    }

    public void OncycleDown()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        cycleDown.AddInteractionPunch(.5f);
        if (raceActive == true)
        {
            raceIndex = (raceIndex + 1) % raceImages.Count;
            mainDisplay.material.mainTexture = raceImages[raceIndex];
        }
        else if (weaponActive == true)
        {
            weaponIndex = (weaponIndex + 1) % weaponImages.Count;
            mainDisplay.material.mainTexture = weaponImages[weaponIndex];
        }
        else if (enemyActive == true)
        {
            enemyIndex = (enemyIndex + 1) % enemyImages.Count;
            mainDisplay.material.mainTexture = enemyImages[enemyIndex];
        }
        else if (cityActive == true)
        {
            cityIndex = (cityIndex + 1) % cityImages.Count;
            mainDisplay.material.mainTexture = cityImages[cityIndex];
        }
        else if (shoutActive == true)
        {
            shoutIndex = (shoutIndex + 1) % shoutOptions.Count;
            mainDisplay.material.mainTexture = screenMat;
            mainShoutText.text = shoutOptions[shoutIndex];
        }
    }

    public void Onsubmit()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        submit.AddInteractionPunch();
        if (solved == true)
        {
            Debug.LogFormat("[Skyrim #{0}] Strike! The module has been disarmed, Dovahkin. Shout no more.", moduleId);
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            if (raceRenderer.material.mainTexture == correctRace && weaponRenderer.material.mainTexture == correctWeapon && enemyRenderer.material.mainTexture == correctEnemy && cityRenderer.material.mainTexture == correctCity && shoutText.text == correctShout)
            {
                Audio.PlaySoundAtTransform("levelUp", transform);
                GetComponent<KMBombModule>().HandlePass();
                Debug.LogFormat("[Skyrim #{0}] Module disarmed, Dovahkin.", moduleId);
                solved = true;
            }
            else
            {
                Debug.LogFormat("[Skyrim #{0}] Strike! You selected {1}, {2}, {3}, {4} & {5} ({6}).", moduleId, raceRenderer.material.mainTexture.name, weaponRenderer.material.mainTexture.name, enemyRenderer.material.mainTexture.name, cityRenderer.material.mainTexture.name, shoutText.text.Replace("\n", "-"), shoutName);
                GetComponent<KMBombModule>().HandleStrike();
            }
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = @"Use “!{0} cycle races weapons enemies cities shouts” to cycle any combination of the screens or “!{0} cycle” to cycle all of them. Use “!{0} set race Altmer”, “!{0} set weapon Bow of the Hunt” etc. to set indivual screens, or use “!{0} weapons” followed by “!{0} up/down” to select an image and “!{0} set” to set it. “!{0} submit” will press the Submit button. You may submit all at once by typing “!{0} submit Argonian, Mace of Molag Bal, Dragon Priest, Whiterun, Ice Form”.";
#pragma warning restore 414

    private bool equalsAnyNoCase(string str, params string[] options)
    {
        return options.Any(opt => opt.Equals(str, StringComparison.InvariantCultureIgnoreCase));
    }

    IEnumerable cycle(KMSelectable screen)
    {
        screen.OnInteract();
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(3f);
            cycleDown.OnInteract();
            yield return "trycancel cycling was aborted by a cancel command.";
        }
        yield return new WaitForSeconds(.3f);
    }

    private KMSelectable nameToSelectable(string name)
    {
        return
            equalsAnyNoCase(name, "r", "race", "races") ? race :
            equalsAnyNoCase(name, "w", "weapon", "weapons") ? weapon :
            equalsAnyNoCase(name, "e", "enemy", "enemies") ? enemy :
            equalsAnyNoCase(name, "c", "city", "cities", "place", "places", "location", "locations") ? city :
            equalsAnyNoCase(name, "s", "shout", "shouts", "call", "calls") ? shout : null;
    }

    private bool isSelected(KMSelectable screen, string input)
    {
        var validOptions =
            screen == shout ? shoutNameOptions : (
                screen == race ? raceImages :
                screen == weapon ? weaponImages :
                screen == enemy ? enemyImages : cityImages
            ).Select(tex => tex.name);
        var curIndex =
            screen == race ? raceIndex :
            screen == weapon ? weaponIndex :
            screen == enemy ? enemyIndex :
            screen == city ? cityIndex : shoutIndex;
        return validOptions.Skip(curIndex).First().Equals(input, StringComparison.InvariantCultureIgnoreCase);
    }

    private IEnumerable attemptToAccept(KMSelectable screen, string input)
    {
        for (int i = 0; i < 3; i++)
        {
            if (isSelected(screen, input))
            {
                accept.OnInteract();
                yield break;
            }
            cycleDown.OnInteract();
            yield return new WaitForSeconds(.1f);
        }

        if (allValidNames.Any(name => name.Equals(input, StringComparison.InvariantCultureIgnoreCase)))
        {
            yield return string.Format("sendtochaterror “{0}” is not one of the {1} on this module.", input, screen == race ? "races" : screen == weapon ? "weapons" : screen == enemy ? "enemies" : screen == city ? "cities" : "dragon shouts");
            yield return "unsubmittablepenalty";
        }
        else
        {
            yield return string.Format("sendtochaterror I don’t know what “{0}” is. Check for typos.", input);
        }
    }

    IEnumerator ProcessTwitchCommand(string command)
    {
        var pieces = command.ToLowerInvariant().Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        KMSelectable screen;

        // Use “!{0} cycle races weapons enemies cities shouts” to cycle any combination of the screens or “!{0} cycle” to cycle all of them.
        if (pieces.Length > 0 && pieces[0].Equals("cycle", StringComparison.InvariantCultureIgnoreCase))
        {
            var cyclers = pieces.Length == 1 ? new[] { race, weapon, enemy, city, shout } : pieces.Skip(1).Select(piece => nameToSelectable(piece)).ToArray();
            if (cyclers.Any(c => c == null))
                yield break;
            yield return null;
            foreach (var cycler in cyclers)
                foreach (var obj in cycle(cycler))
                    yield return obj;
        }
        // Use “!{0} set race Altmer”, “!{0} set weapon Bow of the Hunt” etc. to set indivual screens.
        else if (pieces.Length > 2 && pieces[0].Equals("set", StringComparison.InvariantCultureIgnoreCase) && (screen = nameToSelectable(pieces[1])) != null)
        {
            yield return null;

            screen.OnInteract();
            yield return new WaitForSeconds(.1f);

            // Re-split the original command to get the full input instead of just the first word.
            var input = command.ToLowerInvariant().Trim().Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries)[2];

            foreach (var obj in attemptToAccept(screen, input))
                yield return obj;
        }
        // Use “!{0} weapons” (etc.) to select a screen...
        else if (pieces.Length == 1 && (screen = nameToSelectable(pieces[0])) != null)
        {
            yield return null;
            screen.OnInteract();
        }
        // ... followed by “!{0} up/down” to select an image...
        else if (pieces.Length == 1 && equalsAnyNoCase(pieces[0], "u", "up", "d", "down"))
        {
            yield return null;
            (equalsAnyNoCase(pieces[0], "u", "up") ? cycleUp : cycleDown).OnInteract();
        }
        // ... and “!{0} set” to set it.
        else if (pieces.Length == 1 && equalsAnyNoCase(pieces[0], "set", "accept"))
        {
            yield return null;
            accept.OnInteract();
        }
        // “!{0} submit” will press the Submit button.
        else if ((pieces.Length == 1 && pieces[0].Equals("submit", StringComparison.InvariantCultureIgnoreCase)) ||
            (pieces.Length == 2 && pieces[0].Equals("press", StringComparison.InvariantCultureIgnoreCase) && pieces[1].Equals("submit", StringComparison.InvariantCultureIgnoreCase)))
        {
            yield return null;
            submit.OnInteract();
        }
        // You may submit all at once by typing “!{0} submit Argonian, Mace of Molag Bal, Dragon Priest, Whiterun, Ice Form”.";
        else if (pieces.Length > 1 && pieces[0].Equals("submit", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return null;

            var allInput = command.ToLowerInvariant().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            var inputs = allInput.Split(',').Select(str => str.Trim()).ToArray();
            if (inputs.Length != 5)
            {
                yield return "sendtochaterror You must specify five items separated by commas (race, weapon, enemy, city, dragon shout).";
                yield break;
            }

            var screens = new[] { race, weapon, enemy, city, shout };
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].OnInteract();
                yield return new WaitForSeconds(.1f);

                foreach (var obj in attemptToAccept(screens[i], inputs[i]))
                    yield return obj;

                if (!isSelected(screens[i], inputs[i]))
                    yield break;
            }

            yield return new WaitForSeconds(.1f);
            submit.OnInteract();
        }
    }
}
