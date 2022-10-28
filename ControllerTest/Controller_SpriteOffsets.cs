using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller;
using Model;

namespace ControllerTest;

[TestFixture]
internal class Controller_SpriteOffsets
{
    [TestCase(Character.BabyDaisy)]
    [TestCase(Character.BabyLuigi)]
    [TestCase(Character.BabyMario)]
    [TestCase(Character.BabyPeach)]
    [TestCase(Character.Birdo)]
    [TestCase(Character.Daisy)]
    [TestCase(Character.DiddyKong)]
    [TestCase(Character.DonkeyKong)]
    [TestCase(Character.FunkyKong)]
    [TestCase(Character.DryBowser)]
    [TestCase(Character.DryBones)]
    [TestCase(Character.Toadette)]
    [TestCase(Character.Toad)]
    [TestCase(Character.Bowser)]
    [TestCase(Character.BowserJunior)]
    [TestCase(Character.Luigi)]
    [TestCase(Character.Mario)]
    [TestCase(Character.KoopaTroopa)]
    [TestCase(Character.Peach)]
    [TestCase(Character.Rosalina)]
    [TestCase(Character.KingBoo)]
    [TestCase(Character.Waluigi)]
    [TestCase(Character.Wario)]
    [TestCase(Character.Yoshi)]
    public void TestContains(Character character)
    {
        Assert.That(SpriteOffsets.MinimapOffsets.ContainsKey(character), Is.True);
    }

    [TestCase(Character.BabyDaisy)]
    [TestCase(Character.BabyLuigi)]
    [TestCase(Character.BabyMario)]
    [TestCase(Character.BabyPeach)]
    [TestCase(Character.Birdo)]
    [TestCase(Character.Daisy)]
    [TestCase(Character.DiddyKong)]
    [TestCase(Character.DonkeyKong)]
    [TestCase(Character.FunkyKong)]
    [TestCase(Character.DryBowser)]
    [TestCase(Character.DryBones)]
    [TestCase(Character.Toadette)]
    [TestCase(Character.Toad)]
    [TestCase(Character.Bowser)]
    [TestCase(Character.BowserJunior)]
    [TestCase(Character.Luigi)]
    [TestCase(Character.Mario)]
    [TestCase(Character.KoopaTroopa)]
    [TestCase(Character.Peach)]
    [TestCase(Character.Rosalina)]
    [TestCase(Character.KingBoo)]
    [TestCase(Character.Waluigi)]
    [TestCase(Character.Wario)]
    [TestCase(Character.Yoshi)]
    public void TestNonZeroSize(Character character)
    {
        var offset = SpriteOffsets.MinimapOffsets[character];
        Assert.That(offset.Width, Is.GreaterThan(0));
        Assert.That(offset.Height, Is.GreaterThan(0));
    }
}