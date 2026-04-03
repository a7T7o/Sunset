#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import shutil
from collections import defaultdict
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, Iterable, List, Optional, Tuple

from scene_partial_sync_offline import (
    Doc,
    FILE_ID_RE,
    GAME_OBJECT_RE,
    HEADER_RE,
    SceneData,
    add_child_to_parent,
    remove_child_from_parent,
)

import re


SCRIPT_GUID_RE = re.compile(r"m_Script: \{fileID: 11500000, guid: ([0-9a-f]+), type: 3\}")
GAMEOBJECT_COMPONENTS_BLOCK_RE = re.compile(r"(?ms)^  m_Component:\n(.*?)^  m_Layer:")
GAMEOBJECT_COMPONENT_LINE_RE = re.compile(r"(?m)^  - component: \{fileID: (-?\d+)\}$")


@dataclass(frozen=True)
class DocIdentity:
    kind: str
    path: str
    key: Optional[Tuple[str, object]]


class SceneIndex:
    def __init__(self, scene: SceneData) -> None:
        self.scene = scene
        self.doc_identity: Dict[int, DocIdentity] = {}
        self.component_ids_by_key: Dict[Tuple[str, Tuple[str, object]], List[int]] = defaultdict(list)
        self.component_index_by_id: Dict[int, int] = {}
        self.component_lookup: Dict[Tuple[str, Tuple[str, object], int], int] = {}
        self._build()

    def _build(self) -> None:
        for file_id in self.scene.order:
            doc = self.scene.docs[file_id]
            if doc.class_id == 1:
                transform_id = self.scene.game_object_to_transform.get(file_id)
                if transform_id is None:
                    continue
                path = self.scene.get_transform_path(transform_id)
                self.doc_identity[file_id] = DocIdentity("GameObject", path, None)
                continue

            if doc.class_id in (4, 224):
                path = self.scene.get_transform_path(file_id)
                self.doc_identity[file_id] = DocIdentity("Transform", path, None)
                continue

            game_object_match = GAME_OBJECT_RE.search(doc.text)
            if not game_object_match:
                continue

            game_object_id = int(game_object_match.group(1))
            transform_id = self.scene.game_object_to_transform.get(game_object_id)
            if transform_id is None:
                continue

            path = self.scene.get_transform_path(transform_id)
            if doc.class_id == 114:
                script_match = SCRIPT_GUID_RE.search(doc.text)
                key = ("MonoBehaviour", script_match.group(1) if script_match else "")
            else:
                key = ("Class", doc.class_id)

            self.doc_identity[file_id] = DocIdentity("Component", path, key)
            self.component_ids_by_key[(path, key)].append(file_id)

        for (path, key), component_ids in self.component_ids_by_key.items():
            sorted_component_ids = sorted(component_ids)
            for index, component_id in enumerate(sorted_component_ids):
                self.component_index_by_id[component_id] = index
                self.component_lookup[(path, key, index)] = component_id

    def find_game_object_id(self, path: str) -> Optional[int]:
        transform_id = self.scene.find_transform_by_path(path)
        if transform_id is None:
            return None
        return self.scene.transforms[transform_id].game_object_id

    def find_transform_id(self, path: str) -> Optional[int]:
        return self.scene.find_transform_by_path(path)


def replace_game_object_component_block(text: str, component_ids: List[int]) -> str:
    new_block = "  m_Component:\n"
    for component_id in component_ids:
        new_block += f"  - component: {{fileID: {component_id}}}\n"

    if GAMEOBJECT_COMPONENTS_BLOCK_RE.search(text):
        return GAMEOBJECT_COMPONENTS_BLOCK_RE.sub(lambda _: f"{new_block}  m_Layer:", text, count=1)

    marker = "  m_Layer:"
    index = text.find(marker)
    if index < 0:
        raise ValueError("GameObject 文档缺少 m_Layer，无法更新 m_Component。")
    return text[:index] + new_block + text[index:]


def update_game_object_component_ids(scene: SceneData, game_object_id: int, component_ids: List[int]) -> None:
    game_object_doc = scene.docs[game_object_id]
    rewritten = replace_game_object_component_block(game_object_doc.text, component_ids)
    scene.replace_doc(game_object_id, rewritten)
    scene.reindex()


def rewrite_doc_for_target(
    source_doc: Doc,
    source_index: SceneIndex,
    target_index: SceneIndex,
    target_file_id: int,
    target_game_object_id: Optional[int] = None,
    fail_on_unresolved_scene_refs: bool = True,
) -> Tuple[str, List[str]]:
    unresolved_paths: List[str] = []
    text = HEADER_RE.sub(
        lambda match: f"--- !u!{match.group(1)} &{target_file_id}\n",
        source_doc.text,
        count=1,
    )

    if target_game_object_id is not None:
        game_object_match = GAME_OBJECT_RE.search(text)
        if game_object_match:
            text = GAME_OBJECT_RE.sub(
                f"  m_GameObject: {{fileID: {target_game_object_id}}}",
                text,
                count=1,
            )

    def replace_file_id(match: re.Match[str]) -> str:
        file_id = int(match.group(1))
        if file_id == source_doc.file_id:
            return f"fileID: {target_file_id}"

        identity = source_index.doc_identity.get(file_id)
        if identity is None:
            return match.group(0)

        mapped_id: Optional[int]
        if identity.kind == "GameObject":
            mapped_game_object_id = target_index.find_game_object_id(identity.path)
            mapped_id = mapped_game_object_id
        elif identity.kind == "Transform":
            mapped_id = target_index.find_transform_id(identity.path)
        else:
            assert identity.key is not None
            component_index = source_index.component_index_by_id[file_id]
            mapped_id = target_index.component_lookup.get((identity.path, identity.key, component_index))

        if mapped_id is None:
            unresolved_paths.append(identity.path)
            if fail_on_unresolved_scene_refs:
                return f"fileID: 0"
            return match.group(0)

        return f"fileID: {mapped_id}"

    rewritten = FILE_ID_RE.sub(replace_file_id, text)
    deduped_unresolved = sorted(set(unresolved_paths))
    if deduped_unresolved and fail_on_unresolved_scene_refs:
        raise ValueError(
            f"无法把 {source_index.doc_identity[source_doc.file_id].path} 的场景引用映射到目标场景："
            + ", ".join(deduped_unresolved)
        )

    return rewritten, deduped_unresolved


def sync_transform(
    source: SceneData,
    target: SceneData,
    path: str,
) -> Dict[str, object]:
    source_index = SceneIndex(source)
    target_index = SceneIndex(target)
    source_transform_id = source_index.find_transform_id(path)
    target_transform_id = target_index.find_transform_id(path)
    if source_transform_id is None or target_transform_id is None:
        raise ValueError(f"无法同步 Transform，源或目标缺少路径：{path}")

    source_doc = source.docs[source_transform_id]
    target_game_object_id = target.transforms[target_transform_id].game_object_id
    rewritten, _ = rewrite_doc_for_target(
        source_doc,
        source_index,
        target_index,
        target_transform_id,
        target_game_object_id=target_game_object_id,
    )
    target.replace_doc(target_transform_id, rewritten)
    target.reindex()
    return {
        "path": path,
        "kind": "transform",
        "action": "synced",
    }


def sync_game_object_components(
    source: SceneData,
    target: SceneData,
    path: str,
) -> Dict[str, object]:
    source_index = SceneIndex(source)
    target_index = SceneIndex(target)
    source_transform_id = source_index.find_transform_id(path)
    target_transform_id = target_index.find_transform_id(path)
    if source_transform_id is None or target_transform_id is None:
        raise ValueError(f"无法同步组件，源或目标缺少路径：{path}")

    source_game_object_id = source.transforms[source_transform_id].game_object_id
    target_game_object_id = target.transforms[target_transform_id].game_object_id
    source_game_object = source.game_objects[source_game_object_id]
    target_game_object = target.game_objects[target_game_object_id]

    updated_existing = 0
    added_new = 0
    touched_component_ids = list(target_game_object.component_ids)

    for source_component_id in source_game_object.component_ids:
        source_doc = source.docs[source_component_id]
        if source_doc.class_id in (4, 224):
            continue

        identity = source_index.doc_identity.get(source_component_id)
        if identity is None or identity.kind != "Component" or identity.key is None:
            continue

        component_index = source_index.component_index_by_id[source_component_id]
        target_component_id = target_index.component_lookup.get((path, identity.key, component_index))
        is_new_component = target_component_id is None
        if is_new_component:
            target_component_id = target.max_file_id() + 1

        rewritten, _ = rewrite_doc_for_target(
            source_doc,
            source_index,
            target_index,
            target_component_id,
            target_game_object_id=target_game_object_id,
        )

        if is_new_component:
            target.append_docs([Doc(class_id=source_doc.class_id, file_id=target_component_id, text=rewritten)])
            touched_component_ids.append(target_component_id)
            added_new += 1
        else:
            target.replace_doc(target_component_id, rewritten)
            updated_existing += 1

        target.reindex()
        target_index = SceneIndex(target)

    update_game_object_component_ids(target, target_game_object_id, touched_component_ids)
    return {
        "path": path,
        "kind": "components",
        "updated_existing": updated_existing,
        "added_new": added_new,
    }


def reparent_transform(target: SceneData, child_path: str, new_parent_path: str) -> Dict[str, object]:
    child_transform_id = target.find_transform_by_path(child_path)
    if child_transform_id is None:
        raise ValueError(f"目标场景缺少要重挂的对象：{child_path}")

    new_parent_transform_id = target.find_transform_by_path(new_parent_path)
    if new_parent_transform_id is None:
        raise ValueError(f"目标场景缺少新的父路径：{new_parent_path}")

    child_transform = target.transforms[child_transform_id]
    old_parent_transform_id = child_transform.parent_id
    if old_parent_transform_id == new_parent_transform_id:
        return {
            "kind": "reparent",
            "path": child_path,
            "new_parent": new_parent_path,
            "action": "already-parented",
        }

    if old_parent_transform_id != 0:
        remove_child_from_parent(target, old_parent_transform_id, child_transform_id)
        target.reindex()

    updated_text = re.sub(
        r"(?m)^  m_Father: \{fileID: -?\d+\}$",
        f"  m_Father: {{fileID: {new_parent_transform_id}}}",
        target.docs[child_transform_id].text,
        count=1,
    )
    target.replace_doc(child_transform_id, updated_text)
    target.reindex()

    add_child_to_parent(target, new_parent_transform_id, child_transform_id)
    target.reindex()

    return {
        "kind": "reparent",
        "path": child_path,
        "new_parent": new_parent_path,
        "action": "reparented",
    }


def write_backup(target_path: Path, backup_path: Path) -> None:
    backup_path.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(target_path, backup_path)
    meta_path = target_path.with_suffix(target_path.suffix + ".meta")
    if meta_path.exists():
        shutil.copy2(meta_path, backup_path.with_suffix(backup_path.suffix + ".meta"))


def main() -> int:
    parser = argparse.ArgumentParser(description="离线版 Unity scene 组件/Transform 同步工具。")
    parser.add_argument("--source", required=True, help="源场景路径")
    parser.add_argument("--target", required=True, help="目标场景路径")
    parser.add_argument("--reparent", action="append", default=[], help="把目标里的 childPath 重挂到 newParentPath，格式 childPath=newParentPath")
    parser.add_argument("--sync-go", action="append", default=[], help="同步指定 GameObject 路径上的全部非 Transform 组件，可重复传入")
    parser.add_argument("--sync-transform", action="append", default=[], help="同步指定路径的 Transform，可重复传入")
    parser.add_argument("--backup", help="写入前备份目标场景到指定路径")
    parser.add_argument("--report", help="把执行报告写到指定 JSON")
    parser.add_argument("--dry-run", action="store_true", help="只分析，不落盘")
    args = parser.parse_args()

    source_path = Path(args.source)
    target_path = Path(args.target)
    source = SceneData(source_path)
    target = SceneData(target_path)

    report: Dict[str, object] = {
        "source": str(source_path),
        "target": str(target_path),
        "dry_run": args.dry_run,
        "reparent": args.reparent,
        "sync_go": args.sync_go,
        "sync_transform": args.sync_transform,
        "results": [],
    }

    try:
        if args.backup and not args.dry_run:
            write_backup(target_path, Path(args.backup))
            report["backup"] = args.backup

        for raw in args.reparent:
            if "=" not in raw:
                raise ValueError(f"--reparent 参数格式错误：{raw}")
            child_path, parent_path = raw.split("=", 1)
            result = reparent_transform(target, child_path.strip(), parent_path.strip())
            report["results"].append(result)

        for path in args.sync_transform:
            result = sync_transform(source, target, path)
            report["results"].append(result)

        for path in args.sync_go:
            result = sync_game_object_components(source, target, path)
            report["results"].append(result)

        if not args.dry_run:
            target_path.write_text(target.serialize(), encoding="utf-8")

        verification_scene = target if args.dry_run else SceneData(target_path)
        verification_paths = sorted(
            set(args.sync_go + args.sync_transform + [raw.split("=", 1)[0].strip() for raw in args.reparent])
        )
        report["verification"] = {path: verification_scene.find_transform_by_path(path) is not None for path in verification_paths}
        report["success"] = True
    except Exception as exception:
        report["success"] = False
        report["error"] = f"{type(exception).__name__}: {exception}"

    if args.report:
        report_path = Path(args.report)
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")

    print(json.dumps(report, ensure_ascii=False, indent=2))
    return 0 if report.get("success") else 1


if __name__ == "__main__":
    raise SystemExit(main())
