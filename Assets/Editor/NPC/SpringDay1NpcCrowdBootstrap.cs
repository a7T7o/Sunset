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
    private const string RefreshDialogueMenuPath = "Tools/NPC/Spring Day1/Refresh Crowd Dialogue Assets";
    private const string RefreshManifestMenuPath = "Tools/NPC/Spring Day1/Refresh Crowd Resident Manifest";
    private const string SpriteRoot = "Assets/Sprites/NPC";
    private const string PrefabRoot = "Assets/222_Prefabs/NPC";
    private const string DataRoot = "Assets/111_Data/NPC/SpringDay1Crowd";
    private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";

    private static readonly CastSpec[] Specs =
    {
        new CastSpec(
            "101",
            "ScribeCrowd",
            "村口抄录人",
            "白天常守在村口纸页边，替摊主和邻里抄记零碎口供，外来人一进村时他总会先抬头再低头。",
            "001|NPC001",
            new Vector2(1.6f, -0.55f),
            StoryPhase.EnterVillage,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "你一进村口，半条街的人都先停了手，我这页纸边到现在还压着那阵目光。",
                "有人让开半步，有人只敢隔着人堆偷看，我得先把这阵静记清。",
                "明早摊子照样摆开，可今天落在你身上的那一下停顿，不该就这么散掉。"
            },
            new[]
            {
                "先别急着问路，刚才那阵人群停手看你的样子，我还没记完。",
                "孩子们都躲在人后头偷看，大人倒只会装作自己手里还有活。",
                "你要是听见谁压低嗓子议论，过来和我对一对，我怕他们回头改口。"
            },
            new[]
            {
                "村里最吓人的不是吵，是你一进来时那一下齐齐停手。",
                "等明早大家又照常开摊，这阵目光多半就要装成没发生。"
            },
            new[]
            {
                "所以我才得先把那阵安静记住，免得人一多就只剩嘴硬。",
                "看热闹的人会散，纸上那笔停顿可散不掉。"
            },
            new ConversationSpec(
                "101-ledger-trace",
                Exchange("你一直在记账，是怕村长留下的窟窿没人认吗？", "我看你连边角都抄进去了。", "不是怕没人认，是怕最后每个人都说自己没看见。", "只要纸上还留得住痕，逃掉的人就不算真逃干净。"),
                Exchange("那现在村里谁最值得你信？", "你写这些的时候，心里有怀疑的人吗？", "肯把脏话明着说的人，反而比笑着糊弄人的更可信。", "我先记事实，信任得等风声过去再谈。")),
            Reaction("...", "我先去别处问问，回头再来找你。", "你继续写吧，我不打扰。", "行，回头想起什么再来跟我对一对。", "要是又听见什么零碎动静，下次路过时补我一句。")),

        new CastSpec(
            "102",
            "HunterClue",
            "坡边猎痕人",
            "平时多在村边和河滩认脚印，白天不抢脸，夜里却更容易因为一串乱痕被人注意到。",
            "003|NPC003",
            new Vector2(2.15f, 0.75f),
            StoryPhase.FarmingTutorial,
            StoryPhase.DayEnd,
            Vector2.right,
            new[]
            {
                "夜一深，后坡那段脚步就比白天的话更真。",
                "真有人想装作村里照常转，脚印也会先把心慌露出来。",
                "明早大家照样各站各的位，我还是得先盯昨晚谁绕过了河滩。"
            },
            new[]
            {
                "今晚要往后坡走的话，先听一耳朵风里有没有碎石响。",
                "别看白天大家装得照旧，夜里一乱，脚步就全露底了。",
                "你真想知道那晚谁在看谁，先别把自己脚步踩乱。"
            },
            new[]
            {
                "白天人会装镇定，夜里地面可不会替谁遮掩。",
                "等明早他们照常站好，我照样能从脚印里看出昨晚谁没睡安稳。"
            },
            new[]
            {
                "嗯，所以我宁可多盯两眼后坡，也不信谁嘴上那句没事。",
                "人群白天会散开，夜里的乱痕可没那么快消。"
            },
            new ConversationSpec(
                "102-trail-watch",
                Exchange("你是不是已经找到村长跑走的方向了？", "看你盯着地面很久了。", "大方向知道，可惜对方太会抹痕，像是早把怎么躲人想好了。", "我只差一点点就能咬住后半程。"),
                Exchange("要是我帮你一起看，会不会快一点？", "我路过河边时可以替你瞧一眼。", "会，你眼睛要是够细，能帮我省下半天兜圈子。", "但真碰见不对劲的人，你先顾好自己，别逞强。")),
            Reaction("啧", "我先去前面看看。", "你继续守着这边吧。", "成，别把我这点线索踩散了。", "真有动静的话，我多半比风先听见。")),

        new CastSpec(
            "103",
            "WitnessBoy",
            "跑腿目击少年",
            "平时在屋檐和摊位之间穿梭传话，真有动静时总比大人更早探头，也更容易先看到谁让位谁偷看。",
            "001|NPC001",
            new Vector2(-1.55f, -0.7f),
            StoryPhase.EnterVillage,
            StoryPhase.DayEnd,
            Vector2.down,
            new[]
            {
                "我刚才还踮脚偷看来着，大人一停手，我反倒更想知道你会往哪看。",
                "大家都装没盯着你，可眼睛全从桶沿和门缝后头飘过去了。",
                "明早他们照样站成一排讲话，像今晚谁也没在背后议论过。"
            },
            new[]
            {
                "你是不是也听见他们压低嗓子了？我刚才就在旁边偷听。",
                "别看那些大人嘴硬，刚才你一过来，他们连手里的活都慢了半拍。",
                "村里孩子今天全是又好奇又不敢靠近的样子，我也差不多。"
            },
            new[]
            {
                "我最会看人群什么时候是假装忙，什么时候是真想偷看。",
                "等到明早站位照旧，他们又会装得像昨晚什么都没聊。"
            },
            new[]
            {
                "可不是嘛，嘴上说照常，眼睛还是会往你那边跑。",
                "我先把今晚听见的记牢，免得明早他们又全改口。"
            },
            new ConversationSpec(
                "103-night-back",
                Exchange("你真看见村长跑了？", "那晚到底是什么样？", "真的，他没回头，一路抱着东西往黑里钻。", "我还听见他踩断了一截木栏，像急得怕被谁追上。"),
                Exchange("你为什么还敢把这事说出来？", "换别人早就闭嘴了吧。", "因为我怕再晚一点，连我自己都会怀疑那晚是不是真的。", "我得先把自己看到的说出来，不然转两圈就谁都记不清了。")),
            Reaction("欸", "我先走啦，回头再听你说。", "你继续忙你的，我不耽误你。", "好，那你回来时别装不认识我。", "要是我又想起细节，下回见面再跟你说。")),

        new CastSpec(
            "104",
            "CarpenterCrowd",
            "补门木匠",
            "白天多在门板、栅栏和工作台附近干活，话不多，但谁家门响了、哪面板松了他都先一步知道。",
            "001|NPC001|Anvil_0|Workbench|Anvil",
            new Vector2(-1.35f, 1.1f),
            StoryPhase.WorkbenchFlashback,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "一到晚饭后，敲木声就得轻一点，不然满村都知道谁还没敢回屋。",
                "门栓和窗闩我都多补了一道，真有人半夜心虚，先撞上的也是木板。",
                "明早该开的门还是得照旧开，不能让村里一散就只剩空响。"
            },
            new[]
            {
                "回屋前把门栓再摸一遍，今晚大家心都悬着，响一声都招人侧眼。",
                "刚才饭桌那边一静，外头连敲木头的人都自动把力道压下来了。",
                "等天一亮我还得照旧补栅栏，村里不能因为一个人跑了就全停工。"
            },
            new[]
            {
                "这几晚谁回屋谁让路，我在木头回声里都听得出来。",
                "明早照旧开门干活，才像村里还站得住。"
            },
            new[]
            {
                "对，手上不停，人才不至于盯着那点慌乱越想越大。",
                "门闩先顶住，剩下的议论让他们低声慢慢说。"
            },
            new ConversationSpec(
                "104-board-fix",
                Exchange("你从早忙到晚，是怕村里今晚出事吗？", "这些木板都是刚换上的？", "怕归怕，怕也得先把门补上，不然夜里谁都睡不安。", "村长丢下的烂摊子，总不能让孩子们先顶着。"),
                Exchange("要是我以后来工作台，你可以教我修这些吗？", "我想学点真正能帮上忙的东西。", "可以，先学把手上这一锤落稳，再谈别的。", "会修东西的人，至少不容易被慌乱牵着走。")),
            Reaction("嗯", "我先去忙别的，待会儿再来。", "这些木板先交给你了。", "去吧，路上顺便替我看看哪面栅栏又松了。", "你回来时再听两下，就知道哪面木板又在响了。")),

        new CastSpec(
            "201",
            "MendingCrowd",
            "织补照料人",
            "平时守着针线和布袋，谁袖口裂了、谁脸色不对，她都像本来就在旁边接住。",
            "002|NPC002",
            new Vector2(1.45f, 0.8f),
            StoryPhase.HealingAndHP,
            StoryPhase.DayEnd,
            Vector2.left,
            new[]
            {
                "晚饭后最忙的不是缝口子，是把那些不肯回屋的人一个个劝散。",
                "有人坐下就只敢捏袖口，不说话也知道心里在抖。",
                "明早针线照常摆出来，谁红着眼来，我就当没看见先把口子缝上。"
            },
            new[]
            {
                "你要回屋前先在这儿缓一缓也行，大家今晚说话都压得太低了。",
                "刚才桌上那阵静下来后，连来拿针线的人都不敢把脚步放重。",
                "等天亮了，袖口、布袋、脸色都还得照常收拾，不然村里更容易散。"
            },
            new[]
            {
                "我最怕的不是哭，是大家都装作没事，然后谁也不肯先起身回屋。",
                "明早照样得有人缝口子、递布袋，日子才不会被慌乱拽断。"
            },
            new[]
            {
                "是啊，先把人劝回屋，把气喘匀，夜里就没那么容易胡思乱想。",
                "大家只要还肯照常来找我补东西，村里就还没散。"
            },
            new ConversationSpec(
                "201-seam-breath",
                Exchange("你看起来比别人镇定得多。", "是不是很多人都来找你说话？", "来的人多，慌的人更多，我总得先稳住自己。", "缝衣服时针脚不能乱，安慰人也差不多。"),
                Exchange("村长跑了之后，你最担心什么？", "你心里也会没底吗？", "担心大家嘴上逞强，夜里却一个个睡不着。", "没底当然会有，但只要还有人愿意互相照应，就没到散的时候。")),
            Reaction("好吧", "我先去前面转转。", "你继续忙你的针线。", "去吧，回来时要是手套磨坏了，拿来给我看看。", "回去后别又硬撑到太晚。")),

        new CastSpec(
            "202",
            "HerbCrowd",
            "花草安神人",
            "白天摆花、扎草束，散场后还会把一点亮色留在门边，不让村里只剩火气和灰色。",
            "002|NPC002",
            new Vector2(-1.35f, 0.95f),
            StoryPhase.FarmingTutorial,
            StoryPhase.DayEnd,
            Vector2.right,
            new[]
            {
                "晚饭那阵火气太重，我就把安神草多扎了几束挂在门边。",
                "有人嘴上说不用，手却会在花束边停一下，像怕把夜里的慌带回屋。",
                "明早摊子还是照常摆，我可不想让村里只剩一脸灰色。"
            },
            new[]
            {
                "回屋前拿一小束也好，今晚大家闻见的火药味已经够多了。",
                "别怕我说漂亮话，真到了散场时，总得有人替大家留一点能喘口气的颜色。",
                "天亮后花还得照常开，人也得照常抬头，不然那人才算真把村里吓住了。"
            },
            new[]
            {
                "我不拦人议论，只是不想散场后每扇门里都只剩一股慌味。",
                "明早照常摆花，不是装没事，是提醒大家别把自己也活成枯色。"
            },
            new[]
            {
                "对呀，哪怕只是一小束安神草，也够让人从回屋那几步里缓一口气。",
                "村里要真想撑住，火气旁边总得留点能安神的东西。"
            },
            new ConversationSpec(
                "202-bloom-keep",
                Exchange("你怎么还有心情整理花束？", "大家都在担心的时候，你这儿反而最亮。", "越乱的时候越要有人留一点亮色，不然心会先塌掉。", "花不能替人解决事，可闻见点香，人至少不会一直绷着。"),
                Exchange("这些花是给谁准备的？", "是不是很多人来找你拿安神草？", "给谁都行，睡不着的人、守夜的人、想装镇定的人都需要。", "我只是不想大家一回头，眼里只剩下那人跑掉的事。")),
            Reaction("呀", "我先去别处看看。", "回头再来闻你的花。", "好呀，回来时我给你挑一束不扎手的。", "别走太急，花香跟不上你。")),

        new CastSpec(
            "203",
            "DinnerCrowd",
            "热汤照应人",
            "多半待在灶火和桌边之间，谁空着肚子回屋、谁端碗又默默让位，她都看在眼里。",
            "001|NPC001|Bed|PlayerBed|HomeDoor|House 1_2",
            new Vector2(0.75f, 1.55f),
            StoryPhase.DinnerConflict,
            StoryPhase.DayEnd,
            Vector2.down,
            new[]
            {
                "一提那人跑了，整张桌子先静半口气，只有勺子还在锅边轻轻碰。",
                "嘴上都说不饿，可谁真要回屋前路过这儿，脚步还是会慢下来。",
                "明早我照旧开火，不能让这点热气也跟着昨晚散了。"
            },
            new[]
            {
                "你先别急着走，外头那阵低声议论还没散，垫两口再回屋更稳。",
                "刚才有几个人连碗都端起来了，又因为桌上太静自己默默让开了位。",
                "回屋提醒归提醒，最后真能把人劝住的，还是手里这口热汤。"
            },
            new[]
            {
                "晚饭桌上最沉的不是碗，是谁都不肯先把那句话说破。",
                "等明早照旧开火，大家才有力气继续装镇定或继续追问。"
            },
            new[]
            {
                "可不是嘛，越是没人说话的时候，越得让锅边还有点人气。",
                "先把人喂热，再让他们低声议论去，总好过空着肚子瞎想。"
            },
            new ConversationSpec(
                "203-hot-soup",
                Exchange("你今天看起来比谁都忙。", "是不是很多人来你这儿蹭口热的？", "忙点好，忙着盛汤总比坐着想那些烂事强。", "再说大家都紧绷着，总得有人先把火点起来。"),
                Exchange("你真的不怕村里再乱下去吗？", "夜里要是出事，你会怎么办？", "怕啊，可怕的时候更得让锅里有东西，人才不会先垮。", "真要闹起来，我先把锅守住，别让人饿着胡思乱想。")),
            Reaction("去吧去吧", "我先走一步。", "你这锅汤闻得我更饿了。", "那就别空着肚子乱跑，回来我给你盛。", "今晚想吃热的就来找我。")),
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

    [MenuItem(RefreshDialogueMenuPath)]
    private static void RefreshDialogueAssets()
    {
        EnsureFolders();

        for (int index = 0; index < Specs.Length; index++)
        {
            CreateOrUpdateDialogueContent(Specs[index]);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SpringDay1NpcCrowdBootstrap] 已刷新 {Specs.Length} 份 spring-day1 crowd dialogue assets。");
    }

    [MenuItem(RefreshManifestMenuPath)]
    private static void RefreshManifest()
    {
        EnsureFolders();
        CreateOrUpdateManifest(CreateManifestEntriesFromSpecs());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SpringDay1NpcCrowdBootstrap] 已刷新 {Specs.Length} 名 spring-day1 crowd resident manifest。");
    }

    public static void RunFromCommandLine()
    {
        Bootstrap();
    }

    public static void RefreshDialogueAssetsFromCommandLine()
    {
        RefreshDialogueAssets();
    }

    public static void RefreshManifestFromCommandLine()
    {
        RefreshManifest();
    }

    private static List<SpringDay1NpcCrowdManifest.Entry> CreateManifestEntriesFromSpecs()
    {
        List<SpringDay1NpcCrowdManifest.Entry> manifestEntries = new List<SpringDay1NpcCrowdManifest.Entry>();
        for (int index = 0; index < Specs.Length; index++)
        {
            manifestEntries.Add(Specs[index].CreateManifestEntry());
        }

        return manifestEntries;
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
        SetPrivateField(asset, "phaseSelfTalkLines", BuildPhaseSelfTalkSets(payload.phaseSelfTalkLines));
        SetPrivateField(asset, "playerNearbyLines", payload.playerNearbyLines ?? Array.Empty<string>());
        SetPrivateField(asset, "relationshipStageNearbyLines", BuildRelationshipStageNearbySets(payload.relationshipStageNearbyLines));
        SetPrivateField(asset, "phaseNearbyLines", BuildPhaseNearbySets(payload.phaseNearbyLines));
        SetPrivateField(asset, "defaultInformalConversationBundles", BuildConversationBundles(payload.defaultInformalConversationBundles));
        SetPrivateField(asset, "relationshipStageInformalChatSets", BuildRelationshipStageChatSets(payload.relationshipStageInformalChatSets));
        SetPrivateField(asset, "phaseInformalChatSets", BuildPhaseChatSets(payload.phaseInformalChatSets));
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

    private static NPCDialogueContentProfile.PhaseSelfTalkSet[] BuildPhaseSelfTalkSets(PhaseSelfTalkPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.PhaseSelfTalkSet>();
        }

        NPCDialogueContentProfile.PhaseSelfTalkSet[] results = new NPCDialogueContentProfile.PhaseSelfTalkSet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            PhaseSelfTalkPayload payload = payloads[index];
            NPCDialogueContentProfile.PhaseSelfTalkSet item = new NPCDialogueContentProfile.PhaseSelfTalkSet();
            SetPrivateField(item, "storyPhase", payload?.storyPhase ?? StoryPhase.None);
            SetPrivateField(item, "selfTalkLines", payload?.selfTalkLines ?? Array.Empty<string>());
            item.Sanitize();
            results[index] = item;
        }

        return results;
    }

    private static NPCDialogueContentProfile.PhaseNearbySet[] BuildPhaseNearbySets(PhaseNearbyPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.PhaseNearbySet>();
        }

        NPCDialogueContentProfile.PhaseNearbySet[] results = new NPCDialogueContentProfile.PhaseNearbySet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            PhaseNearbyPayload payload = payloads[index];
            NPCDialogueContentProfile.PhaseNearbySet item = new NPCDialogueContentProfile.PhaseNearbySet();
            SetPrivateField(item, "storyPhase", payload?.storyPhase ?? StoryPhase.None);
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

    private static NPCDialogueContentProfile.PhaseInformalChatSet[] BuildPhaseChatSets(PhaseChatPayload[] payloads)
    {
        if (payloads == null || payloads.Length == 0)
        {
            return Array.Empty<NPCDialogueContentProfile.PhaseInformalChatSet>();
        }

        NPCDialogueContentProfile.PhaseInformalChatSet[] results = new NPCDialogueContentProfile.PhaseInformalChatSet[payloads.Length];
        for (int index = 0; index < payloads.Length; index++)
        {
            PhaseChatPayload payload = payloads[index];
            NPCDialogueContentProfile.PhaseInformalChatSet item = new NPCDialogueContentProfile.PhaseInformalChatSet();
            SetPrivateField(item, "storyPhase", payload?.storyPhase ?? StoryPhase.None);
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
                roamController.SyncRuntimeProfileFromAsset();
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
        public readonly string AssetStem;
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
            AssetStem = ResolveAssetStem(npcId);
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

        public string DialogueContentPath => $"{DataRoot}/NPC_{NpcId}_{AssetStem}DialogueContent.asset";
        public string RoamProfilePath => $"{DataRoot}/NPC_{NpcId}_{AssetStem}RoamProfile.asset";
        public string PrefabPath => $"{PrefabRoot}/{NpcId}.prefab";

        public DialogueContentPayload BuildDialoguePayload()
        {
            return new DialogueContentPayload
            {
                npcId = NpcId,
                selfTalkLines = SelfTalkLines,
                phaseSelfTalkLines = BuildPhaseSelfTalkPayloads(),
                playerNearbyLines = PlayerNearbyLines,
                relationshipStageNearbyLines = Array.Empty<RelationshipStageNearbyPayload>(),
                phaseNearbyLines = BuildPhaseNearbyPayloads(),
                defaultInformalConversationBundles = new[] { Conversation.ToPayload() },
                relationshipStageInformalChatSets = Array.Empty<RelationshipStageChatPayload>(),
                phaseInformalChatSets = BuildPhaseInformalChatPayloads(),
                defaultWalkAwayReaction = WalkAwayReaction.ToPayload(),
                defaultInterruptRules = BuildDefaultInterruptPayloads(),
                defaultResumeRules = BuildDefaultResumePayloads(),
                defaultChatInitiatorLines = AmbientInitiatorLines,
                defaultChatResponderLines = AmbientResponderLines,
                pairDialogueSets = BuildPairDialoguePayloads(NpcId)
            };
        }

        private InterruptRulePayload[] BuildDefaultInterruptPayloads()
        {
            return NpcId switch
            {
                "101" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "先等等，我得先处理一下眼前这件事。",
                        "这句先帮我按住，回头再对。",
                        "行，我先把这页压在手边。",
                        "你忙完回来，我们把刚才缺的那半句补齐。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "先去把那边的话说清楚。",
                        "这页先停在这儿，待会儿再对。",
                        "好，我先把这一笔停住。",
                        "等你腾出空，我们再顺着往下记。")
                },
                "102" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "先等等，我得先处理眼前这桩事。",
                        "你先替我盯住这段乱痕。",
                        "去吧，我先守着这道风口。",
                        "回来时我们继续把那条线拼上。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边的话得先说清，我一会儿再回来。",
                        "你先别散，我还想把这段脚印接上。",
                        "行，我先盯着河滩这头。",
                        "等你回来，我把刚才那阵风再给你捋一遍。")
                },
                "103" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "你先忙，我这回先不抢着往外说。",
                        "那我先把刚想起的那点憋住。",
                        "好，我先把那晚的细节压在心里。",
                        "你回来我再顺着往下说。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "我先去听那边说什么，待会儿再来。",
                        "你别走太快，我还有后半段没讲完。",
                        "行，我先把那晚的路再想一遍。",
                        "等你回来，我就把后坡那段接上。")
                },
                "104" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "先等等，我得先处理眼前这件事。",
                        "你先把这句按在手里。",
                        "去吧，我先把这块木板按稳。",
                        "你回来时，我再把后面那下说给你听。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "先去把那边的话说清。",
                        "木头这边等会儿再接。",
                        "好，我先把锤子落轻一点。",
                        "木板不会自己跑，我们回头再对。")
                },
                "201" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "你先去忙，我这边先替你把气顺住。",
                        "等你忙完了，我们再把刚才那段接上。",
                        "好，你先顾眼前这件事。",
                        "我把这句替你留在针脚边上。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那我先去听那边的正事。",
                        "你这边先别收针。",
                        "行，我先把线头压住。",
                        "你回来时，我们再慢慢把刚才的话缝起来。")
                },
                "202" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "先等等，我得先处理眼前这件事。",
                        "这束花先替我留一下香气。",
                        "去吧，我先把这点亮色看住。",
                        "等你回来，我们再把刚才那句说完。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边要紧，我先去把那头听完。",
                        "你先别把花收起来。",
                        "好，我先让这束花在这儿站着。",
                        "等你回来，我们再继续刚才那阵香气。")
                },
                "203" => new[]
                {
                    InterruptRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "先等等，我得先处理眼前这件事。",
                        "你这锅先替我热着。",
                        "去吧，我先把火看住。",
                        "等你回来，我再把刚才那句给你热一遍。"),
                    InterruptRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边的话得先说清，我一会儿再回来。",
                        "你先别把碗收了。",
                        "行，我先把汤锅稳住。",
                        "等你腾出空，我们再把刚才那口气接上。")
                },
                _ => Array.Empty<InterruptRulePayload>()
            };
        }

        private ResumeRulePayload[] BuildDefaultResumePayloads()
        {
            return NpcId switch
            {
                "101" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，刚才那页我们对到哪了？",
                        "我刚才走开那一下，不想把这页断在半句上。",
                        "好，我把刚才那句翻回来。",
                        "别急，我们从你停下那儿接。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "刚才那边绊住了，我们把这页补完吧。",
                        "正事忙完了，我们继续对刚才那段。",
                        "行，我还把那半句压在纸边。",
                        "好，我们顺着这页往下补。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们继续对刚才那段。",
                        "现在能腾出空了，这页接着来。",
                        "好，我这页正空着等你回来补。",
                        "行，刚才那笔我还没落死。")
                },
                "102" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，河滩那段继续说。",
                        "刚才走开那一下，不想把那串印子断掉。",
                        "好，我们把那串脚印接着对完。",
                        "行，我把刚才那阵风向再捋回来。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "正事忙完了，你刚才那句还能接上吗？",
                        "我回来了，刚才那段乱痕我们继续。",
                        "能，我把风口和脚印都还记着。",
                        "来，我们把那条线重新接牢。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们继续拼那条线。",
                        "现在空出来了，河滩那段接着看。",
                        "嗯，刚才那点痕我还没放掉。",
                        "行，趁风还没换，我们把它说透。")
                },
                "103" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，你刚才那段继续讲吧。",
                        "刚才走开一下，我怕把后坡那段忘了。",
                        "对，我正想起后坡那一下脆响。",
                        "行，我把刚才那截路顺着给你讲。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "我忙完了，你把刚才那句再接给我。",
                        "现在没别的事了，我们把那晚那段讲完。",
                        "好，我还记着那晚的背影怎么拐过去的。",
                        "行，我把刚才憋住的那点再说出来。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，你继续。",
                        "我回来了，刚才那段别断掉。",
                        "行，我把刚才那段顺着给你讲完。",
                        "好，我还记着后坡那边是怎么黑下去的。")
                },
                "104" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，刚才那锤后面的事继续说吧。",
                        "刚才走开一下，木板那段我还想听完。",
                        "好，我把刚才那一下接回去。",
                        "行，咱们从那块松掉的木板往下说。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "正事先忙完了，我们继续刚才那段。",
                        "我回来了，你手上那活不用停。",
                        "好，我手上这块板还按着呢。",
                        "来，我们把刚才那句钉牢。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们回到刚才那块板上。",
                        "现在能接着聊了，刚才那段继续。",
                        "行，我这锤还没落到头。",
                        "好，木头这边的后半句还在。")
                },
                "201" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，我们把刚才那段慢慢接上吧。",
                        "刚才走开一下，不想把你的话丢在半路上。",
                        "好，我还把那句留在针脚边上。",
                        "别急，我们从刚才最稳的地方接起。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "刚才被别的事拽住了，我们继续吧。",
                        "现在空出来了，你刚才那句还能接吗？",
                        "能，我还把那口气替你留着。",
                        "好，我们慢慢把刚才那段缝回去。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们把刚才那段接上。",
                        "我回来了，你刚才那句继续就好。",
                        "行，我先把线头理顺。",
                        "好，刚才那段我还没让它散掉。")
                },
                "202" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，我们把刚才那阵香气接上吧。",
                        "刚才走开一下，不想把那束花晾在半句里。",
                        "好，我还把刚才那点亮色留着。",
                        "来，我们从那束花没说完的地方接。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "刚才先忙别的了，我们继续。",
                        "现在空出来了，那束花还没说完呢。",
                        "好，我还让香气停在这儿。",
                        "行，我们把刚才那点亮色接回去。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们继续刚才那束花。",
                        "我回来了，刚才那句亮色还在吗？",
                        "在，我还把那点香气守着。",
                        "好，我们接着把刚才那句说暖一点。")
                },
                "203" => new[]
                {
                    ResumeRule(
                        NPCInformalChatLeaveCause.DistanceGraceExceeded,
                        NPCInformalChatLeavePhase.Any,
                        "我回来了，刚才那锅热气还没聊完。",
                        "刚才走开一下，不想把那口热汤晾凉。",
                        "好，锅还热着，话也没散。",
                        "来，我们把刚才那口气接回锅边。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.BlockingUi,
                        NPCInformalChatLeavePhase.Any,
                        "正事忙完了，我们继续刚才那段。",
                        "我回来了，那锅边的话还能接上吧？",
                        "能，我还把火看得稳稳的。",
                        "行，我们把刚才那句重新热一遍。"),
                    ResumeRule(
                        NPCInformalChatLeaveCause.DialogueTakeover,
                        NPCInformalChatLeavePhase.Any,
                        "那边说完了，我们继续。",
                        "现在没人插话了，刚才那段接着来。",
                        "好，我这锅还没离火。",
                        "来，把刚才那段重新盛上桌。")
                },
                _ => Array.Empty<ResumeRulePayload>()
            };
        }

        private PhaseSelfTalkPayload[] BuildPhaseSelfTalkPayloads()
        {
            return NpcId switch
            {
                "101" => new[]
                {
                    PhaseSelfTalk(StoryPhase.EnterVillage, "先把围观的眼神记下来，等他们装作没看见时才更好辨。", "纸边先留这一下停顿，村口今天谁抬头最慢我都看得见。"),
                    PhaseSelfTalk(StoryPhase.DinnerConflict, "桌边越装作照常，笔下那点停顿越醒目。", "今晚这口气不能乱记，谁先避开目光得先压在页边。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "先把今晚这页收好，明早谁装没事一翻就知道。", "纸先合上，村口那阵没散掉的安静还得记到天亮。")
                },
                "102" => new[]
                {
                    PhaseSelfTalk(StoryPhase.ReturnAndReminder, "先别把脚印踩乱，回屋前那几步最会露底。", "风向还没换，谁心虚我先从碎石声里记下来。"),
                    PhaseSelfTalk(StoryPhase.FreeTime, "后坡这会儿最诚实，太安静反而不像没事。", "先把呼吸压轻，夜里乱掉的往往不是影子。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "再看一圈地面，没睡稳的人脚步总会发散。", "天亮能埋住一半痕，埋不住昨晚那一下犹豫。")
                },
                "103" => new[]
                {
                    PhaseSelfTalk(StoryPhase.EnterVillage, "先装作没看见他们偷看，回头才更好笑。", "孩子先躲了，大人的眼神反而更会拐弯。"),
                    PhaseSelfTalk(StoryPhase.DinnerConflict, "桌边这阵静得太刻意了，谁压着嗓子我都听得见。", "他们嘴上装没聊你，眼睛倒还在往这边跑。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "今晚谁偷听最久，我到明早都忘不了。", "先把这阵偷看的劲儿记住，明天准有人装作没事。")
                },
                "104" => new[]
                {
                    PhaseSelfTalk(StoryPhase.WorkbenchFlashback, "先把门栓和木缝都看住，响动最会顺着心慌传。", "工作台这边一响，回屋前那点犹豫就藏不住。"),
                    PhaseSelfTalk(StoryPhase.DinnerConflict, "木头敲轻一点，免得把桌边那阵心虚再震出来。", "先补能按稳的地方，夜里门板最怕跟着人心一起松。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "门闩先顶住，天亮了人才站得稳。", "明早照旧开门干活，这一夜的慌才不会越滚越大。")
                },
                "201" => new[]
                {
                    PhaseSelfTalk(StoryPhase.HealingAndHP, "先把针脚压稳，再急的人坐下也得慢一点。", "今晚最怕的不是开口，是谁都憋着不肯先松气。"),
                    PhaseSelfTalk(StoryPhase.DinnerConflict, "针线旁边多留一会儿，乱掉的心总要先找个落点。", "桌边那阵静下去后，来补口子的人反而更多了。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "先把线头理顺，明早大家才不至于一扯就散。", "夜里能把人劝回屋，明天的针脚才有地方落稳。")
                },
                "202" => new[]
                {
                    PhaseSelfTalk(StoryPhase.FarmingTutorial, "先留一点颜色，别让忙出来的灰气跟着回屋。", "安神草不吵，可这会儿正好能把人拉回一点。"),
                    PhaseSelfTalk(StoryPhase.ReturnAndReminder, "回屋前先闻一闻，脚步别让夜色催得太急。", "门边这点香先留着，免得一转身眼里只剩灰。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "花先别收，夜里总得留一点不发硬的亮色。", "明早照常摆出来，村里才不会只剩昨晚那股火气。")
                },
                "203" => new[]
                {
                    PhaseSelfTalk(StoryPhase.DinnerConflict, "锅边这点热气不能断，空着肚子最容易把心也聊凉。", "先让人端稳碗，再听他们把后半句慢慢吐出来。"),
                    PhaseSelfTalk(StoryPhase.ReturnAndReminder, "回屋前再垫两口，别让胡思乱想比热汤先上头。", "火可以收小，人气不能一下散光。"),
                    PhaseSelfTalk(StoryPhase.DayEnd, "明早照旧开火，村里才像还在过日子。", "先把最后回屋的人喂热，再谈别的议论。")
                },
                _ => Array.Empty<PhaseSelfTalkPayload>()
            };
        }

        private PhaseNearbyPayload[] BuildPhaseNearbyPayloads()
        {
            return NpcId switch
            {
                "101" => new[]
                {
                    PhaseNearby(StoryPhase.EnterVillage, "村口这会儿还在拿余光追着你，只是没人肯先抬头。", "你再往前两步，摊边那阵停顿又会慢慢浮出来。"),
                    PhaseNearby(StoryPhase.DinnerConflict, "晚饭那阵过去了，桌边的话更轻，眼神倒没少。", "谁嘴上说照常，手上的动作反而更容易慢半拍。"),
                    PhaseNearby(StoryPhase.DayEnd, "天色收下去后，纸边还压着今晚那阵没散掉的安静。", "明早摊子会照常摆开，但今晚这口气还在村口打转。")
                },
                "102" => new[]
                {
                    PhaseNearby(StoryPhase.ReturnAndReminder, "回屋这一路的脚步最会露底，你先别把碎石踩乱。", "这会儿真心虚的人，往往越接近住处越装得太稳。"),
                    PhaseNearby(StoryPhase.FreeTime, "夜里后坡最诚实，白天压下去的犹豫会从脚步里漏出来。", "你要是也在听动静，就先把自己的步子收轻。"),
                    PhaseNearby(StoryPhase.DayEnd, "收夜后再看一圈，谁没睡安稳其实瞒不过地面。", "天一亮痕会淡，可昨晚那点慌不会这么快全埋住。")
                },
                "103" => new[]
                {
                    PhaseNearby(StoryPhase.EnterVillage, "人堆已经散一点了，可还是有人假装低头偷偷看你。", "孩子们躲回去了，大人反而更爱装作自己刚才没抬头。"),
                    PhaseNearby(StoryPhase.DinnerConflict, "桌边越安静，我越听得出谁把嗓子压得太狠。", "他们现在都装没聊你，可眼神还在拐着弯往这边跑。"),
                    PhaseNearby(StoryPhase.DayEnd, "都这会儿了，刚才那阵偷听的劲儿还没真散掉。", "明早他们会装没事，可今晚谁最爱偷看我记得清。")
                },
                "104" => new[]
                {
                    PhaseNearby(StoryPhase.WorkbenchFlashback, "工作台这边一响，回屋前想多听一句的人脚步就会慢下来。", "门板会响，人心更会响，我这会儿先把能按稳的都按住。"),
                    PhaseNearby(StoryPhase.DinnerConflict, "晚饭后敲木声都压轻了，大家怕再响一声就把心虚抖出来。", "谁回屋前还在门边犹豫，我从木头回声里就听得见。"),
                    PhaseNearby(StoryPhase.DayEnd, "这会儿先把门闩顶稳，比空着手胡思乱想强。", "明早照旧开门干活，村里才不至于被这阵慌乱拖垮。")
                },
                "201" => new[]
                {
                    PhaseNearby(StoryPhase.HealingAndHP, "今晚很多人坐下都先捏袖口，不说话也知道心里在抖。", "你要是想缓一缓就先停这儿，别把气憋着带回屋。"),
                    PhaseNearby(StoryPhase.DinnerConflict, "桌边那阵静下来后，连来找针线的人都把脚步放轻了。", "这会儿先把人稳住，比嘴上逞强有用得多。"),
                    PhaseNearby(StoryPhase.DayEnd, "收夜以后最怕的不是哭，是谁都装作自己没事。", "明早只要还有人来补口子，村里就还没散。")
                },
                "202" => new[]
                {
                    PhaseNearby(StoryPhase.FarmingTutorial, "忙完手上的活也带一束安神草吧，别让慌劲跟着你回屋。", "今晚火气太重了，总得留点颜色把人拉回来。"),
                    PhaseNearby(StoryPhase.ReturnAndReminder, "回屋提醒归提醒，真能让人缓口气的还是门边这点香。", "这会儿大家最怕的不是黑，是一回头眼里只剩灰色。"),
                    PhaseNearby(StoryPhase.DayEnd, "明早花还是照常开，人也得照常抬头。", "村里想撑住，就不能让夜里的慌把颜色全抽干。")
                },
                "203" => new[]
                {
                    PhaseNearby(StoryPhase.DinnerConflict, "外头还在低声议论，你先别空着肚子回屋。", "刚才桌上那半口静气还在，人得靠这点热气把心撑住。"),
                    PhaseNearby(StoryPhase.ReturnAndReminder, "回屋前垫两口更稳，别让夜里的胡思乱想比热汤先上头。", "现在最怕的是人散太快，锅边这点热气也跟着灭了。"),
                    PhaseNearby(StoryPhase.DayEnd, "明早我照旧开火，不能让这点人气跟昨晚一起散掉。", "先把人喂热，剩下那些议论才不至于越聊越冷。")
                },
                _ => Array.Empty<PhaseNearbyPayload>()
            };
        }

        private PhaseChatPayload[] BuildPhaseInformalChatPayloads()
        {
            PhaseChatPayload[] payloads = NpcId switch
            {
                "101" => new[]
                {
                    PhaseChat(
                        StoryPhase.EnterVillage,
                        "101-enter-village-resident",
                        Exchange(
                            "刚才围观散了一点没有？",
                            "他们现在还是一边摆摊一边偷看你吗？",
                            "散是散了，可抬头的次数还比平时多，我这页纸边全记着。",
                            "嘴上都说照常，真到你路过时，手上的活还是会慢半拍。")),
                    PhaseChat(
                        StoryPhase.DinnerConflict,
                        "101-dinner-resident",
                        Exchange(
                            "晚饭那阵静下来后，村里是不是更不好对话了？",
                            "你在桌边那口气里又记下了什么？",
                            "难的不是没人说话，是人人都装作只是低头吃饭。",
                            "我记下的是谁先避开目光，谁又装作只顾着碗边那点热气。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "101-day-end-resident",
                        Exchange(
                            "现在大家都回屋了，你这边还在记今天的事？",
                            "明早这村子真能装成若无其事吗？",
                            "纸页收了，停顿还没收，明早谁照常开口我一听就知道。",
                            "村里会照常转，但今天这一下不可能像没落过一样。"))
                },
                "102" => new[]
                {
                    PhaseChat(
                        StoryPhase.ReturnAndReminder,
                        "102-return-reminder-resident",
                        Exchange(
                            "回屋这一路，你是不是还在看谁的脚步发虚？",
                            "刚才那阵提醒之后，后坡会更安静吗？",
                            "越接近回屋的时候，脚步越容易把心虚露出来。",
                            "安静不代表安全，真乱的人反而会把步子踩得忽快忽慢。")),
                    PhaseChat(
                        StoryPhase.FreeTime,
                        "102-free-time-resident",
                        Exchange(
                            "现在夜里真是你最容易看出破绽的时候？",
                            "后坡那边今晚还有新动静吗？",
                            "夜里地面比人诚实，白天遮过去的犹豫都藏不住。",
                            "只要有人再绕那一脚，我多半先从碎石声里听出来。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "102-day-end-resident",
                        Exchange(
                            "收夜后你还会再去河滩那边看一圈吗？",
                            "到明早这些痕还能留住多少？",
                            "会，夜里最后那一圈往往比白天说得更真。",
                            "天亮会淡掉一部分，但谁昨晚没睡安稳，我还是能看出来。"))
                },
                "103" => new[]
                {
                    PhaseChat(
                        StoryPhase.EnterVillage,
                        "103-enter-village-resident",
                        Exchange(
                            "你刚才在人堆后头都看见谁偷看了？",
                            "村口现在还像刚才那么绷着吗？",
                            "孩子先缩回去了，大人倒还在装作只是顺手抬头。",
                            "绷是绷着，只是没人肯承认自己刚才盯了你多久。")),
                    PhaseChat(
                        StoryPhase.DinnerConflict,
                        "103-dinner-resident",
                        Exchange(
                            "晚饭那会儿你是不是又绕着桌边偷听了一圈？",
                            "大家现在是不是都在装作没聊过你？",
                            "对，越没人先开口，我越能听见谁把嗓子压得最狠。",
                            "他们嘴上装没聊，眼神还在往你那边绕。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "103-day-end-resident",
                        Exchange(
                            "都到这会儿了，你还记着今晚谁说漏了嘴？",
                            "明早他们真会像没事人一样站出来吗？",
                            "记着呢，越是夜里憋回去的话，第二天越会从别处漏出来。",
                            "会装成没事，可谁昨晚偷听最久，我一眼就认得出来。"))
                },
                "104" => new[]
                {
                    PhaseChat(
                        StoryPhase.WorkbenchFlashback,
                        "104-workbench-resident",
                        Exchange(
                            "你在工作台边守到现在，是怕村里哪块又松掉吗？",
                            "这会儿你还听得出谁家门板最不安稳？",
                            "工作台响归响，真正发虚的是那些回屋前还想再听一句的人。",
                            "门板会响，人心更会响，我先把能按稳的都按稳。")),
                    PhaseChat(
                        StoryPhase.DinnerConflict,
                        "104-dinner-resident",
                        Exchange(
                            "晚饭那阵静下来后，你是不是又去看门栓了？",
                            "你这会儿还在补哪一处？",
                            "嗯，那种没人肯先动筷的停顿，比木头开裂还显眼。",
                            "我补的不是一块板，是大家回屋时那点还没稳住的响动。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "104-day-end-resident",
                        Exchange(
                            "都收夜了，你还不回去？",
                            "你觉得明早村里哪处最先恢复照常？",
                            "再看一眼门栓和栅栏，夜里先顶住，明早人才站得稳。",
                            "该开的门还是会照常开，只是有人手上的力道还得慢半拍。"))
                },
                "201" => new[]
                {
                    PhaseChat(
                        StoryPhase.HealingAndHP,
                        "201-healing-resident",
                        Exchange(
                            "疗伤这阵之后，来找你的人是不是更多了？",
                            "大家嘴上不说，脸色你都看出来了吧？",
                            "嗯，越说没事的人，坐下时越会先捏住袖口。",
                            "我先替他们把气顺稳，免得一开口就全是乱的。")),
                    PhaseChat(
                        StoryPhase.DinnerConflict,
                        "201-dinner-resident",
                        Exchange(
                            "晚饭后是不是更多人来你这儿缓口气？",
                            "那阵静下来以后，大家都是什么样子？",
                            "有人只是路过，也会在针线旁多站一会儿。",
                            "最明显的是谁都不想先回屋，可又不敢继续坐在桌边。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "201-day-end-resident",
                        Exchange(
                            "到现在你还在照应别人？",
                            "明早大家真能把今天这口气缝回去吗？",
                            "夜里先把人劝回屋，明早针脚才能落稳。",
                            "能不能全缝平另说，但总得先有人把线头接住。"))
                },
                "202" => new[]
                {
                    PhaseChat(
                        StoryPhase.FarmingTutorial,
                        "202-farming-resident",
                        Exchange(
                            "白天都忙成那样了，你还留着这些花草？",
                            "大家真的会在意这点香气吗？",
                            "越忙越需要一点能让人抬头的颜色，不然心先灰下去。",
                            "嘴上说不在意，路过时还是会慢一下。")),
                    PhaseChat(
                        StoryPhase.ReturnAndReminder,
                        "202-return-reminder-resident",
                        Exchange(
                            "回屋提醒下来后，你是不是更忙着分安神草了？",
                            "这时候他们还会来拿花吗？",
                            "会，越到该回屋的时候，越有人想抓住一点能缓口气的东西。",
                            "不一定真拿走，但闻一闻也够让脚步慢下来。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "202-day-end-resident",
                        Exchange(
                            "收夜以后你还会把花留在门边？",
                            "明早这些亮色还要照常摆出来吗？",
                            "会，夜里总得留一点不那么发硬的颜色。",
                            "要照常摆，村里不能只剩昨晚那股火气。"))
                },
                "203" => new[]
                {
                    PhaseChat(
                        StoryPhase.DinnerConflict,
                        "203-dinner-resident",
                        Exchange(
                            "桌上那阵静下来后，你这边是不是反而更忙了？",
                            "现在还有人绕回来想喝口热的吗？",
                            "更忙，因为谁都不想承认自己其实还想在锅边多站一会儿。",
                            "有，回屋前路过的人总会慢下来，像想再借一口热气压住心里那阵乱。")),
                    PhaseChat(
                        StoryPhase.ReturnAndReminder,
                        "203-return-reminder-resident",
                        Exchange(
                            "提醒大家回屋之后，你这锅火是不是也得慢慢收了？",
                            "这会儿谁最像还没定下心？",
                            "火可以收，人气不能一下收光，不然夜里更空。",
                            "最像没定下心的，往往是端着碗却一直不肯先迈步的那些人。")),
                    PhaseChat(
                        StoryPhase.DayEnd,
                        "203-day-end-resident",
                        Exchange(
                            "都这会儿了，你还守着锅边？",
                            "明早你还打算照旧开火吗？",
                            "守一会儿，免得最后回屋的人连点热气都没接住。",
                            "当然照旧开火，村里得先有人把日子继续点起来。"))
                },
                _ => Array.Empty<PhaseChatPayload>()
            };

            for (int index = 0; index < payloads.Length; index++)
            {
                if (payloads[index] == null)
                {
                    continue;
                }

                payloads[index].walkAwayReaction ??= BuildPhaseWalkAwayReactionPayload(payloads[index].storyPhase);
                if (payloads[index].walkAwayReaction == null)
                {
                    payloads[index].walkAwayReaction = WalkAwayReaction.ToPayload();
                }
            }

            return payloads;
        }

        private InterruptReactionPayload BuildPhaseWalkAwayReactionPayload(StoryPhase storyPhase)
        {
            ReactionSpec reaction = NpcId switch
            {
                "101" => storyPhase switch
                {
                    StoryPhase.EnterVillage => Reaction("101-enter-village-walkaway", "我先顺着村口走一圈。", "先让这些目光自己散一会。", "好，我先把这阵围观看稳。", "你回来时，纸边这一下停顿还在。"),
                    StoryPhase.DinnerConflict => Reaction("101-dinner-walkaway", "我先去桌边外头缓口气。", "先让他们把那口静默咽下去。", "去吧，我先记谁先避开了目光。", "等你回来，这页晚饭的停顿还没翻过去。"),
                    StoryPhase.DayEnd => Reaction("101-day-end-walkaway", "我先回屋歇一会儿。", "先把今晚这口气带回去。", "好，我先把这页收好。", "明早你回来，我们再对谁装作没事。"),
                    _ => null
                },
                "102" => storyPhase switch
                {
                    StoryPhase.ReturnAndReminder => Reaction("102-return-reminder-walkaway", "我先把回屋这段路看一眼。", "先别把碎石声全踩乱。", "去吧，我先盯着谁的脚步发虚。", "你回来时，这条回屋路还会替你说话。"),
                    StoryPhase.FreeTime => Reaction("102-free-time-walkaway", "我先去后坡听听动静。", "这阵夜路我得自己踩一遍。", "好，我先把呼吸压轻。", "真有新痕，我会先替你记住。"),
                    StoryPhase.DayEnd => Reaction("102-day-end-walkaway", "我先回边屋那头看看。", "天亮前我还想再巡一圈。", "去吧，我先把昨晚那点犹豫留住。", "明早你来时，地上的话还没全散。"),
                    _ => null
                },
                "103" => storyPhase switch
                {
                    StoryPhase.EnterVillage => Reaction("103-enter-village-walkaway", "我先混回人堆里转一圈。", "先让他们以为我没在听。", "行，我先装作还在跑腿。", "你回来时，这些偷看的眼神还在打转。"),
                    StoryPhase.DinnerConflict => Reaction("103-dinner-walkaway", "我先绕去桌边外沿看看。", "这阵压着嗓子的劲儿还没散。", "去吧，我先替你听谁又偷聊你。", "等你回来，我把刚才那阵目光再讲给你。"),
                    StoryPhase.DayEnd => Reaction("103-day-end-walkaway", "我先回去歇脚。", "今晚偷听到的够我想一阵了。", "好，我先把这点偷看的劲儿藏住。", "明早你来时，我还记得谁装得最像没事。"),
                    _ => null
                },
                "104" => storyPhase switch
                {
                    StoryPhase.WorkbenchFlashback => Reaction("104-workbench-walkaway", "我先去按一按门栓。", "这边一响，我得先听木头说话。", "去吧，我先把这块松劲看住。", "你回来时，这阵回声还没停。"),
                    StoryPhase.DinnerConflict => Reaction("104-dinner-walkaway", "我先去看看哪块板又在抖。", "桌边这阵静得我想去敲两下木头。", "好，我先把门板的响动压轻。", "你回来时，我还在这儿守着这道缝。"),
                    StoryPhase.DayEnd => Reaction("104-day-end-walkaway", "我先回去收锤子。", "门闩那边还得再顶一顶。", "去吧，我先把夜里的响动按住。", "明早你来时，这条街还是照旧能站稳。"),
                    _ => null
                },
                "201" => storyPhase switch
                {
                    StoryPhase.HealingAndHP => Reaction("201-healing-walkaway", "我先去照看那边的人。", "这会儿总有人还没把气顺过来。", "去吧，我先把这口慌慢慢缝住。", "你回来时，我们再把刚才那句接稳。"),
                    StoryPhase.DinnerConflict => Reaction("201-dinner-walkaway", "我先去看看谁还没回屋。", "桌边那阵静气还压在人肩上。", "好，我先把线头留在这儿。", "等你回来，这段心慌还没散开。"),
                    StoryPhase.DayEnd => Reaction("201-day-end-walkaway", "我先回去收针线。", "夜里这阵得有人替大家按一按。", "去吧，我先把今天的口子留住。", "明早你来时，我们再慢慢把它缝平。"),
                    _ => null
                },
                "202" => storyPhase switch
                {
                    StoryPhase.FarmingTutorial => Reaction("202-farming-walkaway", "我先去换一束安神草。", "这会儿总得留点颜色在手边。", "去吧，我先把花香看住。", "你回来时，这点亮色还替你留着。"),
                    StoryPhase.ReturnAndReminder => Reaction("202-return-reminder-walkaway", "我先去门边再摆一束。", "回屋前总有人还得闻一口。", "好，我先把这点香气留住。", "你回来时，它还会在这儿替人缓气。"),
                    StoryPhase.DayEnd => Reaction("202-day-end-walkaway", "我先回去收花。", "夜里这点颜色不能一下灭掉。", "去吧，我先把不发硬的亮色留一会儿。", "明早你来时，这些花还是照常开。"),
                    _ => null
                },
                "203" => storyPhase switch
                {
                    StoryPhase.DinnerConflict => Reaction("203-dinner-walkaway", "我先去给人盛口热的。", "这阵桌边的静气得靠热汤压一压。", "去吧，我先把锅边这口热气守住。", "你回来时，碗里那点温度还在。"),
                    StoryPhase.ReturnAndReminder => Reaction("203-return-reminder-walkaway", "我先去看最后谁还没垫两口。", "这时候空着肚子最容易乱想。", "好，我先把火收小一点。", "你回来时，人气还没从锅边散光。"),
                    StoryPhase.DayEnd => Reaction("203-day-end-walkaway", "我先回去收火。", "天亮前这锅气还得留一点。", "去吧，我先把最后一口热气看住。", "明早你来时，我还会照旧开火。"),
                    _ => null
                },
                _ => null
            };

            return reaction?.ToPayload();
        }

        public RoamProfilePayload BuildRoamPayload()
        {
            float moveSpeed = NpcId switch
            {
                "102" => 2.62f,
                "103" => 2.72f,
                _ => 2.42f
            };

            float activityRadius = NpcId switch
            {
                "102" => 1.95f,
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
                growthIntent = ResolveGrowthIntent(NpcId),
                residentBaseline = ResolveResidentBaseline(NpcId),
                residentBeatSemantics = ResolveResidentBeatSemantics(NpcId)
            };
        }

        private static SpringDay1CrowdSceneDuty[] ResolveSceneDuties(string npcId)
        {
            return TryGetExistingManifestEntry(npcId, out SpringDay1NpcCrowdManifest.Entry existingEntry) &&
                   existingEntry.sceneDuties != null
                ? (SpringDay1CrowdSceneDuty[])existingEntry.sceneDuties.Clone()
                : Array.Empty<SpringDay1CrowdSceneDuty>();
        }

        private static string[] ResolveSemanticAnchorIds(string npcId)
        {
            return TryGetExistingManifestEntry(npcId, out SpringDay1NpcCrowdManifest.Entry existingEntry) &&
                   existingEntry.semanticAnchorIds != null
                ? (string[])existingEntry.semanticAnchorIds.Clone()
                : Array.Empty<string>();
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
                _ => SpringDay1CrowdGrowthIntent.HoldAnonymous
            };
        }

        private static SpringDay1CrowdResidentBaseline ResolveResidentBaseline(string npcId)
        {
            return TryGetExistingManifestEntry(npcId, out SpringDay1NpcCrowdManifest.Entry existingEntry)
                ? existingEntry.residentBaseline
                : SpringDay1CrowdResidentBaseline.PeripheralResident;
        }

        private static SpringDay1NpcCrowdManifest.ResidentBeatSemantic[] ResolveResidentBeatSemantics(string npcId)
        {
            if (!TryGetExistingManifestEntry(npcId, out SpringDay1NpcCrowdManifest.Entry existingEntry) ||
                existingEntry.residentBeatSemantics == null)
            {
                return Array.Empty<SpringDay1NpcCrowdManifest.ResidentBeatSemantic>();
            }

            SpringDay1NpcCrowdManifest.ResidentBeatSemantic[] source = existingEntry.residentBeatSemantics;
            SpringDay1NpcCrowdManifest.ResidentBeatSemantic[] clones = new SpringDay1NpcCrowdManifest.ResidentBeatSemantic[source.Length];
            for (int index = 0; index < source.Length; index++)
            {
                SpringDay1NpcCrowdManifest.ResidentBeatSemantic semantic = source[index];
                clones[index] = semantic == null
                    ? new SpringDay1NpcCrowdManifest.ResidentBeatSemantic()
                    : new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
                    {
                        beatKey = semantic.beatKey,
                        presenceLevel = semantic.presenceLevel,
                        flags = semantic.flags,
                        note = semantic.note
                    };
            }

            return clones;
        }

        private static bool TryGetExistingManifestEntry(string npcId, out SpringDay1NpcCrowdManifest.Entry entry)
        {
            entry = null;
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return false;
            }

            SpringDay1NpcCrowdManifest manifest = AssetDatabase.LoadAssetAtPath<SpringDay1NpcCrowdManifest>(ManifestPath);
            return manifest != null && manifest.TryGetEntry(npcId.Trim(), out entry) && entry != null;
        }

        private static string ResolveAssetStem(string npcId)
        {
            return npcId switch
            {
                "101" => "LedgerScribe",
                "102" => "Hunter",
                "103" => "ErrandBoy",
                "104" => "Carpenter",
                "201" => "Seamstress",
                "202" => "Florist",
                "203" => "CanteenKeeper",
                _ => throw new ArgumentOutOfRangeException(nameof(npcId), npcId, "未知的 spring-day1 crowd npcId")
            };
        }

        private static SpringDay1NpcCrowdManifest.ResidentBeatSemantic ResidentBeat(
            string beatKey,
            SpringDay1CrowdResidentBeatFlags flags,
            string note)
        {
            return new SpringDay1NpcCrowdManifest.ResidentBeatSemantic
            {
                beatKey = beatKey,
                presenceLevel = ResolveResidentPresenceLevel(flags),
                flags = flags,
                note = note
            };
        }

        private static SpringDay1CrowdResidentPresenceLevel ResolveResidentPresenceLevel(SpringDay1CrowdResidentBeatFlags flags)
        {
            if ((flags & SpringDay1CrowdResidentBeatFlags.AmbientPressure) != 0)
            {
                return SpringDay1CrowdResidentPresenceLevel.Pressure;
            }

            if ((flags & (SpringDay1CrowdResidentBeatFlags.FirstNotice | SpringDay1CrowdResidentBeatFlags.YieldSpace)) != 0)
            {
                return SpringDay1CrowdResidentPresenceLevel.Visible;
            }

            if ((flags & (SpringDay1CrowdResidentBeatFlags.AlreadyAround | SpringDay1CrowdResidentBeatFlags.PretendBusy | SpringDay1CrowdResidentBeatFlags.SilentWatch | SpringDay1CrowdResidentBeatFlags.KeepRoutine)) != 0)
            {
                return SpringDay1CrowdResidentPresenceLevel.Background;
            }

            if ((flags & (SpringDay1CrowdResidentBeatFlags.ReturnHome | SpringDay1CrowdResidentBeatFlags.OffscreenRoutine)) != 0)
            {
                return SpringDay1CrowdResidentPresenceLevel.Trace;
            }

            return SpringDay1CrowdResidentPresenceLevel.None;
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

    [Serializable] private sealed class DialogueContentPayload { public string npcId; public string[] selfTalkLines; public PhaseSelfTalkPayload[] phaseSelfTalkLines; public string[] playerNearbyLines; public RelationshipStageNearbyPayload[] relationshipStageNearbyLines; public PhaseNearbyPayload[] phaseNearbyLines; public ConversationPayload[] defaultInformalConversationBundles; public RelationshipStageChatPayload[] relationshipStageInformalChatSets; public PhaseChatPayload[] phaseInformalChatSets; public InterruptReactionPayload defaultWalkAwayReaction; public InterruptRulePayload[] defaultInterruptRules; public ResumeRulePayload[] defaultResumeRules; public string[] defaultChatInitiatorLines; public string[] defaultChatResponderLines; public PairDialoguePayload[] pairDialogueSets; }
    [Serializable] private sealed class PhaseSelfTalkPayload { public StoryPhase storyPhase; public string[] selfTalkLines; }
    [Serializable] private sealed class RelationshipStageNearbyPayload { public NPCRelationshipStage relationshipStage; public string[] playerNearbyLines; }
    [Serializable] private sealed class PhaseNearbyPayload { public StoryPhase storyPhase; public string[] playerNearbyLines; }
    [Serializable] private sealed class ConversationPayload { public string bundleId; public ExchangePayload[] exchanges; public int completionRelationshipDelta; }
    [Serializable] private sealed class ExchangePayload { public string exchangeId; public string[] playerLines; public string[] npcReplyLines; public float npcReplyDelayMin; public float npcReplyDelayMax; }
    [Serializable] private sealed class RelationshipStageChatPayload { public NPCRelationshipStage relationshipStage; public ConversationPayload[] conversationBundles; public InterruptReactionPayload walkAwayReaction; public InterruptRulePayload[] interruptRules; public ResumeRulePayload[] resumeRules; }
    [Serializable] private sealed class PhaseChatPayload { public StoryPhase storyPhase; public ConversationPayload[] conversationBundles; public InterruptReactionPayload walkAwayReaction; public InterruptRulePayload[] interruptRules; public ResumeRulePayload[] resumeRules; }
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

    private static PhaseChatPayload PhaseChat(StoryPhase storyPhase, string bundleId, params ExchangeSpec[] exchanges)
    {
        return new PhaseChatPayload
        {
            storyPhase = storyPhase,
            conversationBundles = new[] { new ConversationSpec(bundleId, exchanges).ToPayload() },
            walkAwayReaction = null,
            interruptRules = Array.Empty<InterruptRulePayload>(),
            resumeRules = Array.Empty<ResumeRulePayload>()
        };
    }

    private static PhaseNearbyPayload PhaseNearby(StoryPhase storyPhase, params string[] playerNearbyLines)
    {
        return new PhaseNearbyPayload
        {
            storyPhase = storyPhase,
            playerNearbyLines = playerNearbyLines ?? Array.Empty<string>()
        };
    }

    private static PhaseSelfTalkPayload PhaseSelfTalk(StoryPhase storyPhase, params string[] selfTalkLines)
    {
        return new PhaseSelfTalkPayload
        {
            storyPhase = storyPhase,
            selfTalkLines = selfTalkLines ?? Array.Empty<string>()
        };
    }

    private static InterruptRulePayload InterruptRule(
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase,
        string playerExitA,
        string playerExitB,
        string npcReactionA,
        string npcReactionB,
        string cue = "先停一下",
        int relationshipDelta = 0,
        float npcReactionDelay = 0.2f)
    {
        return new InterruptRulePayload
        {
            leaveCause = leaveCause,
            leavePhase = leavePhase,
            reaction = new InterruptReactionPayload
            {
                reactionCue = cue,
                playerExitLines = new[] { playerExitA, playerExitB },
                npcReactionLines = new[] { npcReactionA, npcReactionB },
                relationshipDelta = relationshipDelta,
                npcReactionDelay = npcReactionDelay
            }
        };
    }

    private static ResumeRulePayload ResumeRule(
        NPCInformalChatLeaveCause leaveCause,
        NPCInformalChatLeavePhase leavePhase,
        string playerResumeA,
        string playerResumeB,
        string npcResumeA,
        string npcResumeB,
        float npcResumeDelay = 0.18f)
    {
        return new ResumeRulePayload
        {
            leaveCause = leaveCause,
            leavePhase = leavePhase,
            resumeIntro = new ResumeIntroPayload
            {
                playerResumeLines = new[] { playerResumeA, playerResumeB },
                npcResumeLines = new[] { npcResumeA, npcResumeB },
                npcResumeDelay = npcResumeDelay
            }
        };
    }

    private static PairDialoguePayload[] BuildPairDialoguePayloads(string npcId)
    {
        return npcId switch
        {
            "101" => new[]
            {
                Pair(
                    "103",
                    "刚才人群一停手，你是不是又躲在人后头偷看了？",
                    "你把谁先让开半步、谁只敢探头看清楚点说给我。",
                    "我都看见了，连装忙的人都把手里的活慢了半拍。",
                    "等明早他们照常站好，我都能认出昨晚谁偷看得最久。"),
                Pair(
                    "104",
                    "你补门的木料数我记下了，回头别让人说不清。",
                    "你那边修栅栏的时辰，也许正好能对上村长跑的时间。",
                    "嗯，你先把账记稳，坏掉的门我这边会一块块补回去。",
                    "只要你写得明白，后头追责时就不会只剩吵闹。"),
                Pair(
                    "203",
                    "晚饭桌上那一下静，我也想记在纸边，别让人转头就装没发生。",
                    "你明早照旧开火时，要是还有人半句半句试探，我也想顺手记下来。",
                    "行，你把那阵安静记稳，我这边把热汤和低声议论都看着。",
                    "真有人回屋前又绕回来，多半还是嘴上硬心里乱。")
            },
            "102" => Array.Empty<PairDialoguePayload>(),
            "103" => new[]
            {
                Pair(
                    "101",
                    "我又想起一点，刚才村口那群人是假装忙，其实全在偷看你。",
                    "你快记，谁先让开半步、谁只敢从门缝里看，我都瞧见了。",
                    "我记着呢，你把那阵停手的先后说清，我这边就能补全。",
                    "等明早他们又照常站好，这一晚谁偷看得最久我也认得出来。"),
                Pair(
                    "104",
                    "断掉的那截木栏是不是你后来去补的？",
                    "我那晚听见一声脆响，应该就是那儿。",
                    "对，那道口子我一看就知道不是风刮的。",
                    "你记住响声的位置，我这边就能把缺口对上。"),
                Pair(
                    "203",
                    "那晚我跑回村时闻见你那边还在熬汤。",
                    "我本来想喊人，结果先看见桌边那圈人都压低了声。",
                    "先别光顾着闻汤味，把你看到的人影顺着说一遍。",
                    "你要是又想起细节，来我这儿边喝边讲也行。")
            },
            "104" => new[]
            {
                Pair(
                    "101",
                    "你把谁领过板料记清了，省得后头又有人赖。",
                    "我修门修栅栏，最怕最后连谁动过手都说不明白。",
                    "放心，你这边每一块木料我都给你留着底。",
                    "等账和门都补齐了，谁想糊弄都难。"),
                Pair(
                    "103",
                    "你说的断栏位置我去看过，脚印确实在那儿乱了一次。",
                    "以后你再看见什么，第一时间先来敲我这边的门。",
                    "我知道啦，我这回不只会跑，还会先把地方记住。",
                    "有你去看过，我就更敢说那晚不是我眼花。"),
                Pair(
                    "203",
                    "今晚要是谁不敢回屋，我给你那边再钉两块挡风板。",
                    "桌上那阵太静时，外头这点敲木声反倒能替人壮胆。",
                    "行啊，你把门板钉稳，我这边把回屋前那口热的都给人留着。",
                    "夜里真有人绕回来，先看见的总得是亮堂的火和能挡风的门。")
            },
            "201" => new[]
            {
                Pair(
                    "202",
                    "你把安神草挂近门边些，今晚回屋的人都想装自己没慌。",
                    "等明早照常摆出来，大家看见颜色才肯承认自己一夜没睡好。",
                    "我知道，所以我才不肯让店里只剩灰扑扑的布色。",
                    "你缝住大家的袖口，我来替他们留一点肯抬头的心情。")
            },
            "202" => new[]
            {
                Pair(
                    "201",
                    "我又配了两束不那么苦的安神草，你替我分给今晚不肯回屋的人。",
                    "要是有人嘴硬不肯拿，你就说是我硬塞的。",
                    "好，我一边缝一边替你劝，总有人会肯伸手接。",
                    "你这点亮色和香气，最近比空口安慰管用多了。")
            },
            "203" => new[]
            {
                Pair(
                    "101",
                    "你别又记到忘了抬头，晚饭桌上那阵静我看你也该写一笔。",
                    "你这边纸页一摞高，人就更容易把谁先沉默忘了。",
                    "行，我把这一页收尾就去。",
                    "有你这锅热汤撑着，大家才有力气把低声那点真话继续往外倒。"),
                Pair(
                    "103",
                    "你跑一天也该停下来喝两口，不然回忆都要喘散了。",
                    "别老说自己不饿，我一看就知道你又空着肚子。",
                    "嘿，被你看出来了。",
                    "我喝两口再讲，那晚的路我就能想得更清一点。"),
                Pair(
                    "104",
                    "你那边敲木板敲到这么晚，我这边火也就不敢灭。",
                    "夜里有人听见你敲打的声音，心里反倒会稳一点。",
                    "那你就把火留着，我把能挡风的先全补上。",
                    "村里只要还有热气和敲打声，就不像彻底散了。")
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
