#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sunset.Story;
using UnityEditor;
using UnityEngine;

public static class SpringDay1NpcCrowdBootstrap
{
    private const string MenuPath = "Tools/NPC/Spring Day1/Bootstrap New Cast";
    private const string SpriteRoot = "Assets/Sprites/NPC";
    private const string PrefabRoot = "Assets/222_Prefabs/NPC";
    private const string DataRoot = "Assets/111_Data/NPC/SpringDay1Crowd";
    private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";

    private static readonly CastSpec[] Specs =
    {
        new CastSpec(
            "101",
            "ScribeCrowd",
            "抄录员群众",
            "匿名抄录位，负责承接村内账页与口供碎片，不再 claim 原案正式具名角色。",
            "001|NPC001",
            new Vector2(1.6f, -0.55f),
            StoryPhase.EnterVillage,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "账页缺口越看越大，村长跑得可真会挑时候。",
                "我把印章纹样都抄下来了，谁想赖账都赖不掉。",
                "现在村里最怕的不是乱，是没人肯把实话落在纸上。"
            },
            new[]
            {
                "你也是来问村长那笔烂账的？我这儿能给你一半答案。",
                "别急着站队，先把谁说了什么记清楚。",
                "你要是肯帮忙送几页抄件，我这边会快很多。"
            },
            new[]
            {
                "村里这两天最值钱的不是钱，是有人肯把话说明白。",
                "老账没结，新话又满街跑，写字的人反倒最忙。"
            },
            new[]
            {
                "是啊，我怕大家越传越乱，只好自己一页页补。",
                "真相不见得好听，但总比空着强。"
            },
            new ConversationSpec(
                "101-ledger-trace",
                Exchange("你一直在记账，是怕村长留下的窟窿没人认吗？", "我看你连边角都抄进去了。", "不是怕没人认，是怕最后每个人都说自己没看见。", "只要纸上还留得住痕，逃掉的人就不算真逃干净。"),
                Exchange("那现在村里谁最值得你信？", "你写这些的时候，心里有怀疑的人吗？", "肯把脏话明着说的人，反而比笑着糊弄人的更可信。", "我先记事实，信任得等风声过去再谈。")),
            Reaction("...", "我先去别处问问，回头再来找你。", "你继续写吧，我不打扰。", "行，别忘了回来拿你那一页答案。", "要是听见新消息，记得第一时间告诉我。")),

        new CastSpec(
            "102",
            "HunterClue",
            "猎线索群众",
            "猎线索位，只保留外出追踪与脚印观察语义，不直接 claim 原案正式具名角色。",
            "003|NPC003",
            new Vector2(2.15f, 0.75f),
            StoryPhase.FarmingTutorial,
            StoryPhase.DayEnd,
            Vector2.right,
            new[]
            {
                "脚印到河滩就断了，像是早有人在那儿接应。",
                "村长不是慌着跑的，他走得比野兔还会藏尾巴。",
                "我要是再晚半刻钟追出去，现在就不会只剩这些碎线索。"
            },
            new[]
            {
                "你脚步轻，别踩坏我盯着的这串印子。",
                "想听实话？我不信那老东西是一个人跑的。",
                "要是你往东边去，替我留意有人藏船没有。"
            },
            new[]
            {
                "这村子安静得不对，像狼来之前的林子。",
                "我宁可追空一整天，也不想让那人跑得太轻松。"
            },
            new[]
            {
                "嗯，真要把人追回来，还得靠更多眼睛。",
                "我只信脚印和风向，嘴上的安稳不算数。"
            },
            new ConversationSpec(
                "102-trail-watch",
                Exchange("你是不是已经找到村长跑走的方向了？", "看你盯着地面很久了。", "大方向知道，可惜对方太会抹痕，像老早就排练过。", "我只差一点点就能咬住后半程。"),
                Exchange("要是我帮你一起看，会不会快一点？", "我路过河边时可以替你瞧一眼。", "会，你眼睛要是够细，能帮我省下半天兜圈子。", "但真碰见不对劲的人，你先顾好自己，别逞强。")),
            Reaction("啧", "我先去前面看看。", "你继续守着这边吧。", "成，别把我这点线索踩散了。", "有动静就喊我，我跑得快。")),

        new CastSpec(
            "103",
            "WitnessBoy",
            "目击少年群众",
            "目击型少年群众，只承接跑腿和目击碎片，不再 claim 原案正式具名角色。",
            "001|NPC001",
            new Vector2(-1.55f, -0.7f),
            StoryPhase.EnterVillage,
            StoryPhase.DayEnd,
            Vector2.down,
            new[]
            {
                "我那晚真看到他披着外套往后坡跑，不是眼花。",
                "大家都让我少说，可我憋着又难受。",
                "现在连送封口信的人都找不到，活全压到我头上了。"
            },
            new[]
            {
                "你是不是也想问村长跑那晚的事？我记得可清楚了。",
                "别听那些大人装镇定，他们比我还慌。",
                "要是你去后坡，记得看那棵歪脖树旁边。"
            },
            new[]
            {
                "我现在跑腿都得绕路，谁知道会不会撞上怪东西。",
                "村里话越多，我越怕自己哪句说慢了。"
            },
            new[]
            {
                "我知道，我嘴快，但总得有人把真看到的说出来。",
                "要不是我记得那背影，大家还以为村长是凭空没的。"
            },
            new ConversationSpec(
                "103-night-back",
                Exchange("你真看见村长跑了？", "那晚到底是什么样？", "真的，他没回头，一路抱着东西往黑里钻。", "我还听见他踩断了一截木栏，像急得怕被谁追上。"),
                Exchange("你为什么还敢把这事说出来？", "换别人早就闭嘴了吧。", "因为我怕再晚一点，连我自己都会怀疑那晚是不是真的。", "总得有人把第一眼看到的样子留住，不然大家只会越讲越假。")),
            Reaction("欸", "我先走啦，回头再听你说。", "你继续忙你的，我不耽误你。", "好，那你回来时别装不认识我。", "我这边要是又想起细节，会第一时间告诉你。")),

        new CastSpec(
            "104",
            "CarpenterCrowd",
            "木匠群众",
            "工匠群众位，承接修补与稳定氛围，不再 claim 原案正式具名角色。",
            "001|NPC001|Anvil_0|Workbench|Anvil",
            new Vector2(-1.35f, 1.1f),
            StoryPhase.WorkbenchFlashback,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "门栓得再加一道，村长跑了，胆小鬼也会跟着多起来。",
                "坏的是人心先，木头反倒好修。",
                "这村子要撑住，靠的不是喊话，是一锤一锤补回去。"
            },
            new[]
            {
                "看你像个能搭把手的人，有空就替我搬两块板。",
                "别担心，我这边做的都是让夜里能睡着的活。",
                "要是你今晚要回住处，先看门栓卡没卡紧。"
            },
            new[]
            {
                "手上有活，脑子就不会老去想那些跑掉的人。",
                "能修好的先修，剩下的再慢慢追责。"
            },
            new[]
            {
                "对，我不爱吵，可总得有人把能做的先做完。",
                "村里现在缺的是稳手，不是更大的嗓门。"
            },
            new ConversationSpec(
                "104-board-fix",
                Exchange("你从早忙到晚，是怕村里今晚出事吗？", "这些木板都是刚换上的？", "怕归怕，怕也得先把门补上，不然夜里谁都睡不安。", "村长丢下的烂摊子，总不能让孩子们先顶着。"),
                Exchange("要是我以后来工作台，你可以教我修这些吗？", "我想学点真正能帮上忙的东西。", "可以，先学把手上这一锤落稳，再谈别的。", "会修东西的人，至少不容易被慌乱牵着走。")),
            Reaction("嗯", "我先去忙别的，待会儿再来。", "这些木板先交给你了。", "去吧，路上顺便替我看看哪面栅栏又松了。", "你回来要是还有力气，我再教你下一步。")),

        new CastSpec(
            "201",
            "MendingCrowd",
            "织补群众",
            "织补与照料群众位，承接安抚和缝补语义，不再 claim 原案正式具名角色。",
            "002|NPC002",
            new Vector2(1.45f, 0.8f),
            StoryPhase.HealingAndHP,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "衣角裂了还能补，心里那道口子就难说了。",
                "村长跑后，来找我缝布袋的人一下多了三成。",
                "大家嘴上说没事，手却都在发抖。"
            },
            new[]
            {
                "你脸色还没完全缓过来，先别硬撑。",
                "要是袖口磨破了，来我这儿坐坐，我顺手给你缝上。",
                "最近大家都睡得浅，你也别让自己太累。"
            },
            new[]
            {
                "我最擅长的不是缝线，是让人坐下后慢慢把气喘匀。",
                "有人逃了，有人就得留下来把散掉的地方缝起来。"
            },
            new[]
            {
                "是呀，先把针脚落稳，心才不会跟着散。",
                "要是你也觉得慌，就先在我旁边站一会儿。"
            },
            new ConversationSpec(
                "201-seam-breath",
                Exchange("你看起来比别人镇定得多。", "是不是很多人都来找你说话？", "来的人多，慌的人更多，我总得先稳住自己。", "缝衣服时针脚不能乱，安慰人也差不多。"),
                Exchange("村长跑了之后，你最担心什么？", "你心里也会没底吗？", "担心大家嘴上逞强，夜里却一个个睡不着。", "没底当然会有，但只要还有人愿意互相照应，就没到散的时候。")),
            Reaction("好吧", "我先去前面转转。", "你继续忙你的针线。", "去吧，回来时要是手套磨坏了，拿来给我看看。", "别忘了今晚早点歇。")),

        new CastSpec(
            "202",
            "HerbCrowd",
            "安神草群众",
            "花草与安神草群众位，只保留舒缓和亮色语义，不再 claim 原案正式具名角色。",
            "002|NPC002",
            new Vector2(-1.35f, 0.95f),
            StoryPhase.FarmingTutorial,
            StoryPhase.DayEnd,
            Vector2.right,
            new[]
            {
                "我把安神草和小白花扎在一起，闻起来就没那么苦。",
                "村长跑了也好，至少以后没人拿花束装体面了。",
                "只要花还开着，村里就不该只剩愁眉苦脸。"
            },
            new[]
            {
                "你要不要带一小束花回去？就当给今天压压惊。",
                "别总盯着坏消息看，眼睛会越看越灰。",
                "我知道现在不太像说漂亮话的时候，可人还是得先抬起头。"
            },
            new[]
            {
                "我讨厌再忍忍就好这种话，所以只给人能闻到的安稳。",
                "花开得越亮，我越觉得那个人跑得心虚。"
            },
            new[]
            {
                "嗯，我就是不想让村里一夜之间只剩土色。",
                "真要有人怕黑，我就先把这点亮色守住。"
            },
            new ConversationSpec(
                "202-bloom-keep",
                Exchange("你怎么还有心情整理花束？", "大家都在担心的时候，你这儿反而最亮。", "越乱的时候越要有人留一点亮色，不然心会先塌掉。", "花不会替人解决事，但它能让人记得自己还活着。"),
                Exchange("这些花是给谁准备的？", "是不是很多人来找你拿安神草？", "给谁都行，睡不着的人、守夜的人、想装镇定的人都需要。", "我只是不想让大家一回头，看到的全是逃跑留下的空白。")),
            Reaction("呀", "我先去别处看看。", "回头再来闻你的花。", "好呀，回来时我给你挑一束不扎手的。", "别走太急，花香跟不上你。")),

        new CastSpec(
            "203",
            "DinnerCrowd",
            "饭馆背景群众",
            "饭馆背景群众位，承接热汤与晚餐背景气氛，不再 claim 原案正式核心角色。",
            "001|NPC001|Bed|PlayerBed|HomeDoor|House 1_2",
            new Vector2(0.75f, 1.55f),
            StoryPhase.DinnerConflict,
            StoryPhase.DayEnd,
            Vector2.down,
            new[]
            {
                "今天又多熬了一锅，慌的时候人最容易忘记吃饭。",
                "村长会跑，肚子可不会自己填饱。",
                "嘴上都说不饿，闻见汤味还是会往我这边看。"
            },
            new[]
            {
                "你要是晚点回去，记得先垫两口，别空着肚子扛夜。",
                "我最烦那种跑了还把锅甩给留下人的家伙。",
                "要是你今晚路过我那边，我给你留一碗热的。"
            },
            new[]
            {
                "一锅热汤比十句空安慰管用，我早就知道了。",
                "这村子还能稳住，多半靠的是肯端勺子的人。"
            },
            new[]
            {
                "可不是嘛，真饿的时候连骂人都没力气。",
                "我嘴上凶一点，是怕大家连求助都不好意思开口。"
            },
            new ConversationSpec(
                "203-hot-soup",
                Exchange("你今天看起来比谁都忙。", "是不是很多人来你这儿蹭口热的？", "忙点好，忙着盛汤总比坐着想那些烂事强。", "再说大家都紧绷着，总得有人先把火点起来。"),
                Exchange("你真的不怕村里再乱下去吗？", "夜里要是出事，你会怎么办？", "怕啊，可怕的时候更得让锅里有东西，人才不会先垮。", "要是真出事，我先叫人吃饱，再骂那帮装死的。")),
            Reaction("去吧去吧", "我先走一步。", "你这锅汤闻得我更饿了。", "那就别空着肚子乱跑，回来我给你盛。", "今晚想吃热的就来找我。")),

        new CastSpec(
            "301",
            "NightWitness",
            "夜路见闻位",
            "夜间见闻与传闻氛围位，只保留后坡夜路观察语义，不再作为白天正式角色承载。",
            "003|NPC003|001|NPC001",
            new Vector2(-2.4f, -1.2f),
            StoryPhase.ReturnAndReminder,
            StoryPhase.FreeTime,
            Vector2.up,
            new[]
            {
                "骨头也听见了那晚的脚步，急、虚、还带着怕。",
                "活人逃跑时最吵，偏偏总以为自己没声响。",
                "夜里风从后坡吹下来，带着没来得及埋好的心虚。"
            },
            new[]
            {
                "别离夜路太近，小骨头都知道今晚不太平。",
                "你若找的是跑掉的人，我只能告诉你他怕的比你们想的还重。",
                "回屋前先看看身后，活人的心比骨头更会拐弯。"
            },
            new[]
            {
                "我不追人，我只记得谁踩过坟坡边那条旧路。",
                "夜里最诚实的不是嘴，是脚步。"
            },
            new[]
            {
                "桀桀，不用怕我，怕那些还活着却不敢承认的人就够了。",
                "骨头不说谎，只是说得慢一点。"
            },
            new ConversationSpec(
                "301-bone-night",
                Exchange("你说你听见了那晚的脚步？", "连你都知道村长往哪边跑？", "知道一点，往黑里钻的人最爱踩着以为没人会听见的碎石。", "我不替你们抓人，只替夜里记回声。"),
                Exchange("那我今晚该注意什么？", "夜里真的会更危险吗？", "注意那些忽然太安静的地方，往往比有声响的地方更像陷阱。", "快到屋子时别犹豫，夜色最喜欢咬迟疑的人。")),
            Reaction("咔", "我先回去了。", "夜里这话题听着怪瘆人的。", "回吧回吧，门后面比门外面仁慈。", "别让夜色替你做决定。")),
    };

    [MenuItem(MenuPath)]
    private static void Bootstrap()
    {
        EnsureFolders();
        GeneratePrefabsFromTextures();

        List<SpringDay1NpcCrowdManifest.Entry> manifestEntries = new List<SpringDay1NpcCrowdManifest.Entry>();
        for (int index = 0; index < Specs.Length; index++)
        {
            CastSpec spec = Specs[index];
            NPCDialogueContentProfile content = CreateOrUpdateDialogueContent(spec);
            NPCRoamProfile roamProfile = CreateOrUpdateRoamProfile(spec, content);
            ConfigureGeneratedPrefab(spec, roamProfile);
            manifestEntries.Add(spec.CreateManifestEntry());
        }

        CreateOrUpdateManifest(manifestEntries);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SpringDay1NpcCrowdBootstrap] 已生成 {Specs.Length} 名 spring-day1 新 NPC 的 prefab、profile、对话包与 crowd manifest。");
    }

    public static void RunFromCommandLine()
    {
        Bootstrap();
    }

    private static void GeneratePrefabsFromTextures()
    {
        NPCPrefabGeneratorTool generator = ScriptableObject.CreateInstance<NPCPrefabGeneratorTool>();
        try
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int index = 0; index < Specs.Length; index++)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{SpriteRoot}/{Specs[index].NpcId}.png");
                if (texture == null)
                {
                    throw new FileNotFoundException($"缺少 PNG：{Specs[index].NpcId}.png");
                }

                textures.Add(texture);
            }

            SetPrivateField(generator, "selectedTextures", textures);
            SetPrivateField(generator, "animationRootPath", "Assets/100_Anim/NPC");
            SetPrivateField(generator, "prefabRootPath", PrefabRoot);
            SetPrivateField(generator, "sortingLayerName", "Layer 1");
            SetPrivateField(generator, "defaultMoveSpeed", 2.45f);
            SetPrivateField(generator, "defaultIdleAnimationSpeed", 1f);
            SetPrivateField(generator, "defaultMoveAnimationSpeed", 1f);
            SetPrivateField(generator, "enableDebugLogOnPrefab", false);
            SetPrivateField(generator, "autoAssignBubbleReviewRole", false);

            InvokePrivateMethod(generator, "GenerateNpcAssets");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(generator);
        }
    }

    private static NPCDialogueContentProfile CreateOrUpdateDialogueContent(CastSpec spec)
    {
        string assetPath = spec.DialogueContentPath;
        NPCDialogueContentProfile asset = AssetDatabase.LoadAssetAtPath<NPCDialogueContentProfile>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<NPCDialogueContentProfile>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        DialogueContentPayload payload = spec.BuildDialoguePayload();
        ApplyDialogueContentPayload(asset, payload);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void ApplyDialogueContentPayload(NPCDialogueContentProfile asset, DialogueContentPayload payload)
    {
        SetPrivateField(asset, "npcId", payload.npcId ?? string.Empty);
        SetPrivateField(asset, "selfTalkLines", payload.selfTalkLines ?? Array.Empty<string>());
        SetPrivateField(asset, "playerNearbyLines", payload.playerNearbyLines ?? Array.Empty<string>());
        SetPrivateField(asset, "relationshipStageNearbyLines", BuildRelationshipStageNearbySets(payload.relationshipStageNearbyLines));
        SetPrivateField(asset, "defaultInformalConversationBundles", BuildConversationBundles(payload.defaultInformalConversationBundles));
        SetPrivateField(asset, "relationshipStageInformalChatSets", BuildRelationshipStageChatSets(payload.relationshipStageInformalChatSets));
        SetPrivateField(asset, "defaultWalkAwayReaction", BuildInterruptReaction(payload.defaultWalkAwayReaction));
        SetPrivateField(asset, "defaultInterruptRules", BuildInterruptRules(payload.defaultInterruptRules));
        SetPrivateField(asset, "defaultResumeRules", BuildResumeRules(payload.defaultResumeRules));
        SetPrivateField(asset, "defaultChatInitiatorLines", payload.defaultChatInitiatorLines ?? Array.Empty<string>());
        SetPrivateField(asset, "defaultChatResponderLines", payload.defaultChatResponderLines ?? Array.Empty<string>());
        SetPrivateField(asset, "pairDialogueSets", BuildPairDialogueSets(payload.pairDialogueSets));
        InvokePrivateMethod(asset, "OnValidate");
    }

    private static NPCDialogueContentProfile.RelationshipStageNearbySet[] BuildRelationshipStageNearbySets(RelationshipStageNearbyPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.RelationshipStageNearbySet>();
        }

        NPCDialogueContentProfile.RelationshipStageNearbySet[] results = new NPCDialogueContentProfile.RelationshipStageNearbySet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            RelationshipStageNearbyPayload payload = payloads[index];
            NPCDialogueContentProfile.RelationshipStageNearbySet item = new NPCDialogueContentProfile.RelationshipStageNearbySet();
            SetPrivateField(item, "relationshipStage", payload?.relationshipStage ?? NPCRelationshipStage.Stranger);
            SetPrivateField(item, "playerNearbyLines", payload?.playerNearbyLines ?? Array.Empty<string>());
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCDialogueContentProfile.InformalConversationBundle[] BuildConversationBundles(ConversationPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.InformalConversationBundle>();
        }

        NPCDialogueContentProfile.InformalConversationBundle[] results = new NPCDialogueContentProfile.InformalConversationBundle[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            ConversationPayload payload = payloads[index];
            NPCDialogueContentProfile.InformalConversationBundle bundle = new NPCDialogueContentProfile.InformalConversationBundle();
            SetPrivateField(bundle, "bundleId", payload?.bundleId ?? string.Empty);
            SetPrivateField(bundle, "exchanges", BuildExchanges(payload?.exchanges));
            SetPrivateField(bundle, "completionRelationshipDelta", payload?.completionRelationshipDelta ?? 0);
            bundle.Sanitize();
            results[index] = bundle;
        }

        return results;
    }

    private static NPCDialogueContentProfile.InformalChatExchange[] BuildExchanges(ExchangePayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.InformalChatExchange>();
        }

        NPCDialogueContentProfile.InformalChatExchange[] results = new NPCDialogueContentProfile.InformalChatExchange[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            ExchangePayload payload = payloads[index];
            NPCDialogueContentProfile.InformalChatExchange exchange = new NPCDialogueContentProfile.InformalChatExchange();
            SetPrivateField(exchange, "exchangeId", payload?.exchangeId ?? string.Empty);
            SetPrivateField(exchange, "playerLines", payload?.playerLines ?? Array.Empty<string>());
            SetPrivateField(exchange, "npcReplyLines", payload?.npcReplyLines ?? Array.Empty<string>());
            SetPrivateField(exchange, "npcReplyDelayMin", payload?.npcReplyDelayMin ?? 0f);
            SetPrivateField(exchange, "npcReplyDelayMax", payload?.npcReplyDelayMax ?? 0f);
            exchange.Sanitize();
            results[index] = exchange;
        }

        return results;
    }

    private static NPCDialogueContentProfile.RelationshipStageInformalChatSet[] BuildRelationshipStageChatSets(RelationshipStageChatPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.RelationshipStageInformalChatSet>();
        }

        NPCDialogueContentProfile.RelationshipStageInformalChatSet[] results = new NPCDialogueContentProfile.RelationshipStageInformalChatSet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            RelationshipStageChatPayload payload = payloads[index];
            NPCDialogueContentProfile.RelationshipStageInformalChatSet item = new NPCDialogueContentProfile.RelationshipStageInformalChatSet();
            SetPrivateField(item, "relationshipStage", payload?.relationshipStage ?? NPCRelationshipStage.Stranger);
            SetPrivateField(item, "conversationBundles", BuildConversationBundles(payload?.conversationBundles));
            SetPrivateField(item, "walkAwayReaction", BuildInterruptReaction(payload?.walkAwayReaction));
            SetPrivateField(item, "interruptRules", BuildInterruptRules(payload?.interruptRules));
            SetPrivateField(item, "resumeRules", BuildResumeRules(payload?.resumeRules));
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCDialogueContentProfile.InformalChatInterruptReaction BuildInterruptReaction(InterruptReactionPayload payload)
    {
        return payload == null
            ? NPCDialogueContentProfile.InformalChatInterruptReaction.Create(string.Empty, Array.Empty<string>(), Array.Empty<string>(), 0, 0.45f)
            : NPCDialogueContentProfile.InformalChatInterruptReaction.Create(
                payload.reactionCue,
                payload.playerExitLines ?? Array.Empty<string>(),
                payload.npcReactionLines ?? Array.Empty<string>(),
                payload.relationshipDelta,
                payload.npcReactionDelay);
    }

    private static NPCDialogueContentProfile.InformalChatInterruptRule[] BuildInterruptRules(InterruptRulePayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.InformalChatInterruptRule>();
        }

        NPCDialogueContentProfile.InformalChatInterruptRule[] results = new NPCDialogueContentProfile.InformalChatInterruptRule[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            InterruptRulePayload payload = payloads[index];
            NPCDialogueContentProfile.InformalChatInterruptRule item = new NPCDialogueContentProfile.InformalChatInterruptRule();
            SetPrivateField(item, "leaveCause", payload?.leaveCause ?? NPCInformalChatLeaveCause.Any);
            SetPrivateField(item, "leavePhase", payload?.leavePhase ?? NPCInformalChatLeavePhase.Any);
            SetPrivateField(item, "reaction", BuildInterruptReaction(payload?.reaction));
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCDialogueContentProfile.InformalChatResumeIntro BuildResumeIntro(ResumeIntroPayload payload)
    {
        return payload == null
            ? NPCDialogueContentProfile.InformalChatResumeIntro.Create(Array.Empty<string>(), Array.Empty<string>(), 0.18f)
            : NPCDialogueContentProfile.InformalChatResumeIntro.Create(
                payload.playerResumeLines ?? Array.Empty<string>(),
                payload.npcResumeLines ?? Array.Empty<string>(),
                payload.npcResumeDelay);
    }

    private static NPCDialogueContentProfile.InformalChatResumeRule[] BuildResumeRules(ResumeRulePayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.InformalChatResumeRule>();
        }

        NPCDialogueContentProfile.InformalChatResumeRule[] results = new NPCDialogueContentProfile.InformalChatResumeRule[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            ResumeRulePayload payload = payloads[index];
            NPCDialogueContentProfile.InformalChatResumeRule item = new NPCDialogueContentProfile.InformalChatResumeRule();
            SetPrivateField(item, "leaveCause", payload?.leaveCause ?? NPCInformalChatLeaveCause.Any);
            SetPrivateField(item, "leavePhase", payload?.leavePhase ?? NPCInformalChatLeavePhase.Any);
            SetPrivateField(item, "resumeIntro", BuildResumeIntro(payload?.resumeIntro));
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCDialogueContentProfile.PairDialogueSet[] BuildPairDialogueSets(PairDialoguePayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.PairDialogueSet>();
        }

        NPCDialogueContentProfile.PairDialogueSet[] results = new NPCDialogueContentProfile.PairDialogueSet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            PairDialoguePayload payload = payloads[index];
            NPCDialogueContentProfile.PairDialogueSet item = new NPCDialogueContentProfile.PairDialogueSet();
            SetPrivateField(item, "partnerNpcId", payload?.partnerNpcId ?? string.Empty);
            SetPrivateField(item, "initiatorLines", payload?.initiatorLines ?? Array.Empty<string>());
            SetPrivateField(item, "responderLines", payload?.responderLines ?? Array.Empty<string>());
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCRoamProfile CreateOrUpdateRoamProfile(CastSpec spec, NPCDialogueContentProfile content)
    {
        string assetPath = spec.RoamProfilePath;
        NPCRoamProfile asset = AssetDatabase.LoadAssetAtPath<NPCRoamProfile>(assetPath);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<NPCRoamProfile>();
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        RoamProfilePayload payload = spec.BuildRoamPayload();
        EditorJsonUtility.FromJsonOverwrite(JsonUtility.ToJson(payload), asset);

        SerializedObject serializedObject = new SerializedObject(asset);
        serializedObject.FindProperty("dialogueContentProfile").objectReferenceValue = content;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void ConfigureGeneratedPrefab(CastSpec spec, NPCRoamProfile roamProfile)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(spec.PrefabPath);
        try
        {
            NPCAutoRoamController roamController = prefabRoot.GetComponent<NPCAutoRoamController>();
            if (roamController != null)
            {
                SerializedObject serializedObject = new SerializedObject(roamController);
                serializedObject.FindProperty("roamProfile").objectReferenceValue = roamProfile;
                serializedObject.FindProperty("applyProfileOnAwake").boolValue = true;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                roamController.ApplyProfile();
            }

            if (prefabRoot.GetComponent<NPCDialogueInteractable>() == null
                && prefabRoot.GetComponent<NPCInformalChatInteractable>() == null)
            {
                prefabRoot.AddComponent<NPCInformalChatInteractable>();
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, spec.PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static void CreateOrUpdateManifest(List<SpringDay1NpcCrowdManifest.Entry> entries)
    {
        SpringDay1NpcCrowdManifest manifest = AssetDatabase.LoadAssetAtPath<SpringDay1NpcCrowdManifest>(ManifestPath);
        if (manifest == null)
        {
            manifest = ScriptableObject.CreateInstance<SpringDay1NpcCrowdManifest>();
            AssetDatabase.CreateAsset(manifest, ManifestPath);
        }

        manifest.SetEntries(entries.ToArray());
        EditorUtility.SetDirty(manifest);
    }

    private static void EnsureFolders()
    {
        EnsureFolder(DataRoot);
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/Story");
        EnsureFolder("Assets/Resources/Story/SpringDay1");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string leaf = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, leaf);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new MissingFieldException(target.GetType().FullName, fieldName);
        }

        field.SetValue(target, value);
    }

    private static void InvokePrivateMethod(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException(target.GetType().FullName, methodName);
        }

        method.Invoke(target, null);
    }

    [Serializable]
    private sealed class CastSpec
    {
        public readonly string NpcId;
        public readonly string Slug;
        public readonly string DisplayName;
        public readonly string RoleSummary;
        public readonly string AnchorObjectName;
        public readonly Vector2 SpawnOffset;
        public readonly StoryPhase MinPhase;
        public readonly StoryPhase MaxPhase;
        public readonly Vector2 InitialFacing;
        public readonly string[] SelfTalkLines;
        public readonly string[] PlayerNearbyLines;
        public readonly string[] AmbientInitiatorLines;
        public readonly string[] AmbientResponderLines;
        public readonly ConversationSpec Conversation;
        public readonly ReactionSpec WalkAwayReaction;

        public CastSpec(
            string npcId,
            string slug,
            string displayName,
            string roleSummary,
            string anchorObjectName,
            Vector2 spawnOffset,
            StoryPhase minPhase,
            StoryPhase maxPhase,
            Vector2 initialFacing,
            string[] selfTalkLines,
            string[] playerNearbyLines,
            string[] ambientInitiatorLines,
            string[] ambientResponderLines,
            ConversationSpec conversation,
            ReactionSpec walkAwayReaction)
        {
            NpcId = npcId;
            Slug = slug;
            DisplayName = displayName;
            RoleSummary = roleSummary;
            AnchorObjectName = anchorObjectName;
            SpawnOffset = spawnOffset;
            MinPhase = minPhase;
            MaxPhase = maxPhase;
            InitialFacing = initialFacing;
            SelfTalkLines = selfTalkLines;
            PlayerNearbyLines = playerNearbyLines;
            AmbientInitiatorLines = ambientInitiatorLines;
            AmbientResponderLines = ambientResponderLines;
            Conversation = conversation;
            WalkAwayReaction = walkAwayReaction;
        }

        public string DialogueContentPath => $"{DataRoot}/NPC_{NpcId}_{Slug}DialogueContent.asset";
        public string RoamProfilePath => $"{DataRoot}/NPC_{NpcId}_{Slug}RoamProfile.asset";
        public string PrefabPath => $"{PrefabRoot}/{NpcId}.prefab";

        public DialogueContentPayload BuildDialoguePayload()
        {
            return new DialogueContentPayload
            {
                npcId = NpcId,
                selfTalkLines = SelfTalkLines,
                playerNearbyLines = PlayerNearbyLines,
                relationshipStageNearbyLines = Array.Empty<RelationshipStageNearbyPayload>(),
                defaultInformalConversationBundles = new[] { Conversation.ToPayload() },
                relationshipStageInformalChatSets = Array.Empty<RelationshipStageChatPayload>(),
                defaultWalkAwayReaction = WalkAwayReaction.ToPayload(),
                defaultInterruptRules = Array.Empty<InterruptRulePayload>(),
                defaultResumeRules = Array.Empty<ResumeRulePayload>(),
                defaultChatInitiatorLines = AmbientInitiatorLines,
                defaultChatResponderLines = AmbientResponderLines,
                pairDialogueSets = BuildPairDialoguePayloads(NpcId)
            };
        }

        public RoamProfilePayload BuildRoamPayload()
        {
            float moveSpeed = NpcId switch
            {
                "102" => 2.62f,
                "103" => 2.72f,
                "301" => 2.18f,
                _ => 2.42f
            };

            float activityRadius = NpcId switch
            {
                "102" => 1.95f,
                "301" => 1.25f,
                _ => 1.7f
            };

            return new RoamProfilePayload
            {
                moveSpeed = moveSpeed,
                idleAnimationSpeed = 1f,
                moveAnimationSpeed = 1f,
                activityRadius = activityRadius,
                minimumMoveDistance = Mathf.Min(0.6f, activityRadius * 0.45f),
                pathSampleAttempts = 12,
                shortPauseMin = 0.8f,
                shortPauseMax = 2.4f,
                shortPauseCountMin = 2,
                shortPauseCountMax = 4,
                longPauseMin = 3.2f,
                longPauseMax = 5.8f,
                stuckCheckInterval = 0.3f,
                stuckDistanceThreshold = 0.08f,
                maxStuckRecoveries = 3,
                enableAmbientChat = true,
                ambientChatRadius = 3.1f,
                ambientChatChance = 0.6f,
                ambientChatResponseDelay = 0.82f,
                selfTalkLines = SelfTalkLines,
                chatInitiatorLines = AmbientInitiatorLines,
                chatResponderLines = AmbientResponderLines
            };
        }

        public SpringDay1NpcCrowdManifest.Entry CreateManifestEntry()
        {
            return new SpringDay1NpcCrowdManifest.Entry
            {
                npcId = NpcId,
                displayName = DisplayName,
                roleSummary = RoleSummary,
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath),
                anchorObjectName = AnchorObjectName,
                spawnOffset = SpawnOffset,
                fallbackWorldPosition = Vector2.zero,
                initialFacing = InitialFacing,
                minPhase = MinPhase,
                maxPhase = MaxPhase,
                sceneDuties = ResolveSceneDuties(NpcId),
                semanticAnchorIds = ResolveSemanticAnchorIds(NpcId),
                growthIntent = ResolveGrowthIntent(NpcId)
            };
        }

        private static SpringDay1CrowdSceneDuty[] ResolveSceneDuties(string npcId)
        {
            return npcId switch
            {
                "101" => new[]
                {
                    SpringDay1CrowdSceneDuty.EnterVillagePostEntryCrowd,
                    SpringDay1CrowdSceneDuty.DinnerBackground,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "102" => new[]
                {
                    SpringDay1CrowdSceneDuty.NightWitness,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "103" => new[]
                {
                    SpringDay1CrowdSceneDuty.EnterVillagePostEntryCrowd,
                    SpringDay1CrowdSceneDuty.EnterVillageKidLook,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "104" => new[]
                {
                    SpringDay1CrowdSceneDuty.DinnerBackground,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "201" => new[]
                {
                    SpringDay1CrowdSceneDuty.DinnerBackground,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "202" => new[]
                {
                    SpringDay1CrowdSceneDuty.DinnerBackground,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "203" => new[]
                {
                    SpringDay1CrowdSceneDuty.DinnerBackground,
                    SpringDay1CrowdSceneDuty.DailyStand
                },
                "301" => new[]
                {
                    SpringDay1CrowdSceneDuty.NightWitness
                },
                _ => Array.Empty<SpringDay1CrowdSceneDuty>()
            };
        }

        private static string[] ResolveSemanticAnchorIds(string npcId)
        {
            return npcId switch
            {
                "101" => new[] { "EnterVillageCrowdRoot", "DinnerBackgroundRoot", "DailyStand_01" },
                "102" => new[] { "NightWitness_01", "DailyStand_03" },
                "103" => new[] { "EnterVillageCrowdRoot", "KidLook_01", "DailyStand_02" },
                "104" => new[] { "DinnerBackgroundRoot", "DailyStand_01" },
                "201" => new[] { "DinnerBackgroundRoot", "DailyStand_02" },
                "202" => new[] { "DinnerBackgroundRoot", "DailyStand_03" },
                "203" => new[] { "DinnerBackgroundRoot", "DailyStand_01" },
                "301" => new[] { "NightWitness_01" },
                _ => Array.Empty<string>()
            };
        }

        private static SpringDay1CrowdGrowthIntent ResolveGrowthIntent(string npcId)
        {
            return npcId switch
            {
                "101" => SpringDay1CrowdGrowthIntent.UpgradeCandidate,
                "102" => SpringDay1CrowdGrowthIntent.UpgradeCandidate,
                "103" => SpringDay1CrowdGrowthIntent.UpgradeCandidate,
                "104" => SpringDay1CrowdGrowthIntent.StableSupport,
                "201" => SpringDay1CrowdGrowthIntent.HoldAnonymous,
                "202" => SpringDay1CrowdGrowthIntent.HoldAnonymous,
                "203" => SpringDay1CrowdGrowthIntent.StableSupport,
                "301" => SpringDay1CrowdGrowthIntent.UpgradeCandidate,
                _ => SpringDay1CrowdGrowthIntent.HoldAnonymous
            };
        }
    }

    [Serializable]
    private sealed class ConversationSpec
    {
        public readonly string BundleId;
        public readonly ExchangeSpec[] Exchanges;

        public ConversationSpec(string bundleId, params ExchangeSpec[] exchanges)
        {
            BundleId = bundleId;
            Exchanges = exchanges ?? Array.Empty<ExchangeSpec>();
        }

        public ConversationPayload ToPayload()
        {
            ExchangePayload[] exchangePayloads = new ExchangePayload[Exchanges.Length];
            for (int index = 0; index < Exchanges.Length; index++)
            {
                exchangePayloads[index] = Exchanges[index].ToPayload(index);
            }

            return new ConversationPayload
            {
                bundleId = BundleId,
                exchanges = exchangePayloads,
                completionRelationshipDelta = 1
            };
        }
    }

    [Serializable]
    private sealed class ExchangeSpec
    {
        public readonly string[] PlayerLines;
        public readonly string[] NpcReplyLines;

        public ExchangeSpec(string[] playerLines, string[] npcReplyLines)
        {
            PlayerLines = playerLines;
            NpcReplyLines = npcReplyLines;
        }

        public ExchangePayload ToPayload(int index)
        {
            return new ExchangePayload
            {
                exchangeId = $"exchange-{index + 1}",
                playerLines = PlayerLines,
                npcReplyLines = NpcReplyLines,
                npcReplyDelayMin = 0.52f,
                npcReplyDelayMax = 0.86f
            };
        }
    }

    [Serializable]
    private sealed class ReactionSpec
    {
        public readonly string ReactionCue;
        public readonly string[] PlayerExitLines;
        public readonly string[] NpcReactionLines;

        public ReactionSpec(string reactionCue, string playerExitA, string playerExitB, string npcReactionA, string npcReactionB)
        {
            ReactionCue = reactionCue;
            PlayerExitLines = new[] { playerExitA, playerExitB };
            NpcReactionLines = new[] { npcReactionA, npcReactionB };
        }

        public InterruptReactionPayload ToPayload()
        {
            return new InterruptReactionPayload
            {
                reactionCue = ReactionCue,
                playerExitLines = PlayerExitLines,
                npcReactionLines = NpcReactionLines,
                relationshipDelta = 0,
                npcReactionDelay = 0.4f
            };
        }
    }

    [Serializable] private sealed class DialogueContentPayload { public string npcId; public string[] selfTalkLines; public string[] playerNearbyLines; public RelationshipStageNearbyPayload[] relationshipStageNearbyLines; public ConversationPayload[] defaultInformalConversationBundles; public RelationshipStageChatPayload[] relationshipStageInformalChatSets; public InterruptReactionPayload defaultWalkAwayReaction; public InterruptRulePayload[] defaultInterruptRules; public ResumeRulePayload[] defaultResumeRules; public string[] defaultChatInitiatorLines; public string[] defaultChatResponderLines; public PairDialoguePayload[] pairDialogueSets; }
    [Serializable] private sealed class RelationshipStageNearbyPayload { public NPCRelationshipStage relationshipStage; public string[] playerNearbyLines; }
    [Serializable] private sealed class ConversationPayload { public string bundleId; public ExchangePayload[] exchanges; public int completionRelationshipDelta; }
    [Serializable] private sealed class ExchangePayload { public string exchangeId; public string[] playerLines; public string[] npcReplyLines; public float npcReplyDelayMin; public float npcReplyDelayMax; }
    [Serializable] private sealed class RelationshipStageChatPayload { public NPCRelationshipStage relationshipStage; public ConversationPayload[] conversationBundles; public InterruptReactionPayload walkAwayReaction; public InterruptRulePayload[] interruptRules; public ResumeRulePayload[] resumeRules; }
    [Serializable] private sealed class InterruptReactionPayload { public string reactionCue; public string[] playerExitLines; public string[] npcReactionLines; public int relationshipDelta; public float npcReactionDelay; }
    [Serializable] private sealed class InterruptRulePayload { public NPCInformalChatLeaveCause leaveCause; public NPCInformalChatLeavePhase leavePhase; public InterruptReactionPayload reaction; }
    [Serializable] private sealed class ResumeRulePayload { public NPCInformalChatLeaveCause leaveCause; public NPCInformalChatLeavePhase leavePhase; public ResumeIntroPayload resumeIntro; }
    [Serializable] private sealed class ResumeIntroPayload { public string[] playerResumeLines; public string[] npcResumeLines; public float npcResumeDelay; }
    [Serializable] private sealed class PairDialoguePayload { public string partnerNpcId; public string[] initiatorLines; public string[] responderLines; }
    [Serializable] private sealed class RoamProfilePayload { public float moveSpeed; public float idleAnimationSpeed; public float moveAnimationSpeed; public float activityRadius; public float minimumMoveDistance; public int pathSampleAttempts; public float shortPauseMin; public float shortPauseMax; public int shortPauseCountMin; public int shortPauseCountMax; public float longPauseMin; public float longPauseMax; public float stuckCheckInterval; public float stuckDistanceThreshold; public int maxStuckRecoveries; public bool enableAmbientChat; public float ambientChatRadius; public float ambientChatChance; public float ambientChatResponseDelay; public string[] selfTalkLines; public string[] chatInitiatorLines; public string[] chatResponderLines; }

    private static ExchangeSpec Exchange(string playerA, string playerB, string npcA, string npcB)
    {
        return new ExchangeSpec(new[] { playerA, playerB }, new[] { npcA, npcB });
    }

    private static ReactionSpec Reaction(string cue, string playerExitA, string playerExitB, string npcReactionA, string npcReactionB)
    {
        return new ReactionSpec(cue, playerExitA, playerExitB, npcReactionA, npcReactionB);
    }

    private static PairDialoguePayload[] BuildPairDialoguePayloads(string npcId)
    {
        return npcId switch
        {
            "101" => new[]
            {
                Pair(
                    "103",
                    "阿澈，你把那晚看到的背影再说慢一点，我怕抄漏。",
                    "你跑得快，但这回得让每个细节都落在纸上。",
                    "别急，我正把你说的歪脖树和断栏写在同一页。",
                    "你记得越准，村长那条夜路就越藏不住。"),
                Pair(
                    "104",
                    "沈斧，你补门的木料数我记下了，回头别让人说不清。",
                    "你那边修栅栏的时辰，也许正好能对上村长跑的时间。",
                    "嗯，你先把账记稳，坏掉的门我这边会一块块补回去。",
                    "只要你写得明白，后头追责时就不会只剩吵闹。"),
                Pair(
                    "203",
                    "麦禾，今晚多熬的那锅也记一笔，村里现在连热汤都成安稳了。",
                    "你那边谁半夜来讨汤，我也想顺手记个名。",
                    "你先写，我这边把人喂饱了，大家才肯把实话吐出来。",
                    "真要有人夜里慌得睡不着，最后还是会端着碗来找我。")
            },
            "102" => new[]
            {
                Pair(
                    "301",
                    "朽钟，你守夜路这么久，后坡那串碎石到底往哪边拐了？",
                    "我追白天的脚印，你记夜里的回声，我们该能拼出一条线。",
                    "我听见的方向和你猜的差不离，只是那人比风还心虚。",
                    "你盯白天，我看黑路，迟早把他怕的地方挖出来。")
            },
            "103" => new[]
            {
                Pair(
                    "101",
                    "莱札，我又想起一点，那人翻过栏的时候外套角是湿的。",
                    "你快记，我怕待会儿又被别人打岔。",
                    "我记着呢，你只管把先后顺序说清。",
                    "你想到一点，我这边就能把那晚补完整一点。"),
                Pair(
                    "104",
                    "沈斧叔，断掉的那截木栏是不是你后来去补的？",
                    "我那晚听见一声脆响，应该就是那儿。",
                    "对，那道口子我一看就知道不是风刮的。",
                    "你记住响声的位置，我这边就能把缺口对上。"),
                Pair(
                    "203",
                    "麦禾姐，那晚我跑回村时闻见你那边还在熬汤。",
                    "我本来想喊人，结果先被那股热气绊住了。",
                    "先别光顾着闻汤味，把你看到的人影顺着说一遍。",
                    "你要是又想起细节，来我这儿边喝边讲也行。")
            },
            "104" => new[]
            {
                Pair(
                    "101",
                    "莱札，你把谁领过板料记清了，省得后头又有人赖。",
                    "我修门修栅栏，最怕最后连谁动过手都说不明白。",
                    "放心，你这边每一块木料我都给你留着底。",
                    "等账和门都补齐了，谁想糊弄都难。"),
                Pair(
                    "103",
                    "阿澈，你说的断栏位置我去看过，脚印确实在那儿乱了一次。",
                    "以后你再看见什么，第一时间先来敲我这边的门。",
                    "我知道啦，我这回不只会跑，还会先把地方记住。",
                    "有你去看过，我就更敢说那晚不是我眼花。"),
                Pair(
                    "203",
                    "麦禾，今晚要是还有人守夜，我给你那边再钉两块挡风板。",
                    "人心一乱，能挡风挡雨的地方就更不能漏。",
                    "行啊，你把门板钉稳，我这边把热汤一直热着。",
                    "夜里真要有人跑来求个安稳，先看到的总得是亮堂堂的灶火。")
            },
            "201" => new[]
            {
                Pair(
                    "202",
                    "桃羽，把安神草扎紧些，今晚来找我的手都在抖。",
                    "你那束亮色一摆上去，人坐下时气就顺多了。",
                    "我知道，所以我才不肯让店里只剩灰扑扑的布色。",
                    "你缝住大家的袖口，我来替他们留一点肯抬头的心情。")
            },
            "202" => new[]
            {
                Pair(
                    "201",
                    "白槿姐，我又配了两束不那么苦的安神草，你替我分给睡不着的人。",
                    "要是有人嘴硬不肯拿，你就说是我硬塞的。",
                    "好，我一边缝一边替你劝，总有人会肯伸手接。",
                    "你这点亮色和香气，最近比空口安慰管用多了。")
            },
            "203" => new[]
            {
                Pair(
                    "101",
                    "莱札，你别又写到忘了时辰，晚点我给你送碗热的。",
                    "你这边纸页一摞高，人就更容易把肚子忘了。",
                    "行，我把这一页收尾就去。",
                    "有你那锅热汤撑着，大家才有力气把实话继续往外倒。"),
                Pair(
                    "103",
                    "阿澈，你跑一天也该停下来喝两口，不然回忆都要喘散了。",
                    "别老说自己不饿，我一看就知道你又空着肚子。",
                    "嘿，被你看出来了。",
                    "我喝两口再讲，那晚的路我就能想得更清一点。"),
                Pair(
                    "104",
                    "沈斧，你那边敲木板敲到这么晚，我这边火也就不敢灭。",
                    "夜里有人听见你敲打的声音，心里反倒会稳一点。",
                    "那你就把火留着，我把能挡风的先全补上。",
                    "村里只要还有热气和敲打声，就不像彻底散了。")
            },
            "301" => new[]
            {
                Pair(
                    "102",
                    "炎栎，别只盯着地上，夜里的回声比脚印更会告密。",
                    "那人踩过坟坡边时心跳乱得像敲坏了钟。",
                    "嗯，你盯黑里那一截，我来追能看见的那半程。",
                    "只要回声和脚印对上，他再会藏也藏不住。")
            },
            _ => Array.Empty<PairDialoguePayload>()
        };
    }

    private static PairDialoguePayload Pair(
        string partnerNpcId,
        string initiatorA,
        string initiatorB,
        string responderA,
        string responderB)
    {
        return new PairDialoguePayload
        {
            partnerNpcId = partnerNpcId,
            initiatorLines = new[] { initiatorA, initiatorB },
            responderLines = new[] { responderA, responderB }
        };
    }
}
#endif
