#!/usr/bin/env python3
from __future__ import annotations

import argparse
import codecs
import json
import re
import shutil
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, Iterable, List, Optional, Set, Tuple


DOC_SPLIT_RE = re.compile(r"(?m)^--- !u!(\d+) &(-?\d+)\n")
FILE_ID_RE = re.compile(r"fileID: (-?\d+)")
HEADER_RE = re.compile(r"^--- !u!(\d+) &(-?\d+)\n")
NAME_RE = re.compile(r"(?m)^  m_Name: (.*)$")
GAME_OBJECT_RE = re.compile(r"(?m)^  m_GameObject: \{fileID: (-?\d+)\}$")
PREFAB_INSTANCE_RE = re.compile(r"(?m)^  m_PrefabInstance: \{fileID: (-?\d+)\}$")
COMPONENT_RE = re.compile(r"(?m)^  - component: \{fileID: (-?\d+)\}$")
FATHER_RE = re.compile(r"(?m)^  m_Father: \{fileID: (-?\d+)\}$")
CHILDREN_BLOCK_RE = re.compile(r"(?ms)^  m_Children:\n(.*?)^  m_Father:")
CHILD_RE = re.compile(r"(?m)^  - \{fileID: (-?\d+)\}$")


@dataclass
class Doc:
    class_id: int
    file_id: int
    text: str


@dataclass
class GameObjectInfo:
    file_id: int
    name: str
    component_ids: List[int]


@dataclass
class TransformInfo:
    file_id: int
    game_object_id: int
    parent_id: int
    child_ids: List[int]


class SceneData:
    def __init__(self, path: Path) -> None:
        self.path = path
        self.text = path.read_text(encoding="utf-8")
        self.prefix, self.docs, self.order = self._parse_docs(self.text)
        self.game_objects: Dict[int, GameObjectInfo] = {}
        self.transforms: Dict[int, TransformInfo] = {}
        self.transform_to_path: Dict[int, str] = {}
        self.path_to_transform: Dict[str, int] = {}
        self.normalized_path_to_transform: Dict[str, int] = {}
        self.game_object_to_transform: Dict[int, int] = {}
        self.reindex()

    @staticmethod
    def _parse_docs(text: str) -> Tuple[str, Dict[int, Doc], List[int]]:
        matches = list(DOC_SPLIT_RE.finditer(text))
        if not matches:
            raise ValueError("未找到 Unity scene 文档分块，输入不是可解析的 YAML scene。")

        prefix = text[: matches[0].start()]
        docs: Dict[int, Doc] = {}
        order: List[int] = []
        for index, match in enumerate(matches):
            start = match.start()
            end = matches[index + 1].start() if index + 1 < len(matches) else len(text)
            doc_text = text[start:end]
            class_id = int(match.group(1))
            file_id = int(match.group(2))
            docs[file_id] = Doc(class_id=class_id, file_id=file_id, text=doc_text)
            order.append(file_id)
        return prefix, docs, order

    def _build_index(self) -> None:
        for file_id in self.order:
            doc = self.docs[file_id]
            if doc.class_id == 1:
                name_match = NAME_RE.search(doc.text)
                name = name_match.group(1) if name_match else ""
                component_ids = [int(component_id) for component_id in COMPONENT_RE.findall(doc.text)]
                self.game_objects[file_id] = GameObjectInfo(
                    file_id=file_id,
                    name=name,
                    component_ids=component_ids,
                )
            elif doc.class_id in (4, 224):
                game_object_match = GAME_OBJECT_RE.search(doc.text)
                if not game_object_match:
                    raise ValueError(f"Transform 文档缺少 m_GameObject: {file_id}")
                game_object_id = int(game_object_match.group(1))
                parent_match = FATHER_RE.search(doc.text)
                parent_id = int(parent_match.group(1)) if parent_match else 0
                children_match = CHILDREN_BLOCK_RE.search(doc.text)
                child_ids = [int(child_id) for child_id in CHILD_RE.findall(children_match.group(1) if children_match else "")]
                self.transforms[file_id] = TransformInfo(
                    file_id=file_id,
                    game_object_id=game_object_id,
                    parent_id=parent_id,
                    child_ids=child_ids,
                )
                self.game_object_to_transform[game_object_id] = file_id

        root_transforms = [transform.file_id for transform in self.transforms.values() if transform.parent_id == 0]
        for transform_id in root_transforms:
            self._index_path(transform_id, "")

    def reindex(self) -> None:
        self.game_objects.clear()
        self.transforms.clear()
        self.transform_to_path.clear()
        self.path_to_transform.clear()
        self.normalized_path_to_transform.clear()
        self.game_object_to_transform.clear()
        self._build_index()

    def _index_path(self, transform_id: int, parent_path: str) -> None:
        transform = self.transforms[transform_id]
        game_object = self.game_objects[transform.game_object_id]
        current_path = f"{parent_path}/{game_object.name}" if parent_path else game_object.name
        self.transform_to_path[transform_id] = current_path
        self.path_to_transform[current_path] = transform_id
        self.normalized_path_to_transform.setdefault(normalize_path_key(current_path), transform_id)
        for child_transform_id in transform.child_ids:
            if child_transform_id in self.transforms:
                self._index_path(child_transform_id, current_path)

    def get_transform_path(self, transform_id: int) -> str:
        return self.transform_to_path[transform_id]

    def find_transform_by_path(self, path: str) -> Optional[int]:
        exact = self.path_to_transform.get(path)
        if exact is not None:
            return exact
        return self.normalized_path_to_transform.get(normalize_path_key(path))

    def collect_subtree_doc_ids(self, root_transform_id: int) -> Set[int]:
        collected: Set[int] = set()

        def walk_transform(transform_id: int) -> None:
            transform = self.transforms[transform_id]
            collected.add(transform.file_id)
            game_object = self.game_objects[transform.game_object_id]
            collected.add(game_object.file_id)
            for component_id in game_object.component_ids:
                collected.add(component_id)
            for child_transform_id in transform.child_ids:
                walk_transform(child_transform_id)

        walk_transform(root_transform_id)
        return collected

    def assert_plain_scene_subtree(self, doc_ids: Iterable[int]) -> None:
        for doc_id in doc_ids:
            doc = self.docs[doc_id]
            prefab_match = PREFAB_INSTANCE_RE.search(doc.text)
            if prefab_match and int(prefab_match.group(1)) != 0:
                raise ValueError(
                    f"{self.path} 中所选对象树包含 PrefabInstance（fileID={doc_id}），当前离线版只支持纯场景对象。"
                )

    def max_file_id(self) -> int:
        return max(self.docs.keys())

    def replace_doc(self, file_id: int, text: str) -> None:
        if file_id not in self.docs:
            raise KeyError(f"未找到要替换的文档 fileID={file_id}")
        self.docs[file_id] = Doc(class_id=self.docs[file_id].class_id, file_id=file_id, text=text)

    def append_docs(self, docs: List[Doc]) -> None:
        for doc in docs:
            self.docs[doc.file_id] = doc
            self.order.append(doc.file_id)

    def remove_docs(self, file_ids: Set[int]) -> None:
        self.order = [file_id for file_id in self.order if file_id not in file_ids]
        for file_id in file_ids:
            self.docs.pop(file_id, None)

    def serialize(self) -> str:
        parts = [self.prefix]
        for index, file_id in enumerate(self.order):
            text = self.docs[file_id].text
            if index > 0 and not parts[-1].endswith("\n"):
                parts.append("\n")
            parts.append(text)
            if not text.endswith("\n"):
                parts.append("\n")
        return "".join(parts)


def get_parent_path(path: str) -> str:
    if "/" not in path:
        return ""
    return path.rsplit("/", 1)[0]


def normalize_path_key(path: str) -> str:
    if not path:
        return ""
    segments: List[str] = []
    for segment in path.split("/"):
        cleaned = segment.strip()
        if len(cleaned) >= 2 and cleaned[0] == '"' and cleaned[-1] == '"':
            cleaned = cleaned[1:-1]
        if "\\u" in cleaned or "\\x" in cleaned:
            try:
                cleaned = codecs.decode(cleaned, "unicode_escape")
            except Exception:
                pass
        segments.append(cleaned)
    return "/".join(segments)


def rewrite_doc_ids(text: str, id_map: Dict[int, int]) -> str:
    header_match = HEADER_RE.match(text)
    if not header_match:
        raise ValueError("文档头无法解析。")

    old_file_id = int(header_match.group(2))
    new_file_id = id_map[old_file_id]
    text = HEADER_RE.sub(lambda match: f"--- !u!{match.group(1)} &{new_file_id}\n", text, count=1)

    def replace_file_id(match: re.Match[str]) -> str:
        file_id = int(match.group(1))
        return f"fileID: {id_map.get(file_id, file_id)}"

    return FILE_ID_RE.sub(replace_file_id, text)


def replace_transform_parent(text: str, parent_id: int) -> str:
    return FATHER_RE.sub(f"  m_Father: {{fileID: {parent_id}}}", text, count=1)


def update_children_block(text: str, child_ids: List[int]) -> str:
    new_block = "  m_Children:\n"
    for child_id in child_ids:
        new_block += f"  - {{fileID: {child_id}}}\n"

    if CHILDREN_BLOCK_RE.search(text):
        return CHILDREN_BLOCK_RE.sub(lambda _: f"{new_block}  m_Father:", text, count=1)

    marker = "  m_Father:"
    index = text.find(marker)
    if index < 0:
        raise ValueError("Transform 文档缺少 m_Father，无法更新 m_Children。")
    return text[:index] + new_block + text[index:]


def add_child_to_parent(scene: SceneData, parent_transform_id: int, child_transform_id: int) -> None:
    transform = scene.transforms[parent_transform_id]
    child_ids = list(transform.child_ids)
    if child_transform_id not in child_ids:
        child_ids.append(child_transform_id)
    updated_text = update_children_block(scene.docs[parent_transform_id].text, child_ids)
    scene.replace_doc(parent_transform_id, updated_text)
    scene.transforms[parent_transform_id] = TransformInfo(
        file_id=transform.file_id,
        game_object_id=transform.game_object_id,
        parent_id=transform.parent_id,
        child_ids=child_ids,
    )


def remove_child_from_parent(scene: SceneData, parent_transform_id: int, child_transform_id: int) -> None:
    transform = scene.transforms[parent_transform_id]
    child_ids = [candidate for candidate in transform.child_ids if candidate != child_transform_id]
    updated_text = update_children_block(scene.docs[parent_transform_id].text, child_ids)
    scene.replace_doc(parent_transform_id, updated_text)
    scene.transforms[parent_transform_id] = TransformInfo(
        file_id=transform.file_id,
        game_object_id=transform.game_object_id,
        parent_id=transform.parent_id,
        child_ids=child_ids,
    )


def clone_subtree_safe(
    source: SceneData,
    target: SceneData,
    path: str,
    mode: str,
) -> Dict[str, object]:
    source_root_transform_id = source.find_transform_by_path(path)
    if source_root_transform_id is None:
        raise ValueError(f"源场景缺少路径：{path}")

    source_doc_ids = source.collect_subtree_doc_ids(source_root_transform_id)
    source.assert_plain_scene_subtree(source_doc_ids)

    existing_target_root_transform_id = target.find_transform_by_path(path)
    if existing_target_root_transform_id is not None and mode == "copy-missing":
        return {
            "path": path,
            "action": "skipped-existing",
            "copied_docs": 0,
        }

    parent_path = get_parent_path(path)
    target_parent_transform_id = 0
    if parent_path:
        parent_transform_id = target.find_transform_by_path(parent_path)
        if parent_transform_id is None:
            raise ValueError(f"目标场景缺少父路径，无法挂载：{path}")
        target_parent_transform_id = parent_transform_id

    if existing_target_root_transform_id is not None and mode == "overwrite":
        existing_parent_id = target.transforms[existing_target_root_transform_id].parent_id
        if existing_parent_id != 0:
            remove_child_from_parent(target, existing_parent_id, existing_target_root_transform_id)
        target_doc_ids = target.collect_subtree_doc_ids(existing_target_root_transform_id)
        target.remove_docs(target_doc_ids)
        target.reindex()

    max_target_id = target.max_file_id()
    id_map: Dict[int, int] = {}
    next_id = max_target_id + 1
    for old_id in sorted(source_doc_ids):
        id_map[old_id] = next_id
        next_id += 1

    cloned_docs: List[Doc] = []
    for old_id in sorted(source_doc_ids):
        source_doc = source.docs[old_id]
        rewritten_text = rewrite_doc_ids(source_doc.text, id_map)
        cloned_docs.append(Doc(class_id=source_doc.class_id, file_id=id_map[old_id], text=rewritten_text))

    cloned_root_transform_id = id_map[source_root_transform_id]
    for index, doc in enumerate(cloned_docs):
        if doc.file_id == cloned_root_transform_id:
            cloned_docs[index] = Doc(
                class_id=doc.class_id,
                file_id=doc.file_id,
                text=replace_transform_parent(doc.text, target_parent_transform_id),
            )
            break

    target.append_docs(cloned_docs)
    target.reindex()
    if target_parent_transform_id != 0:
        add_child_to_parent(target, target_parent_transform_id, cloned_root_transform_id)
        target.reindex()

    return {
        "path": path,
        "action": "copied" if existing_target_root_transform_id is None else "overwritten",
        "copied_docs": len(cloned_docs),
    }


def write_backup(target_path: Path, backup_path: Path) -> None:
    backup_path.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(target_path, backup_path)
    meta_path = target_path.with_suffix(target_path.suffix + ".meta")
    if meta_path.exists():
        shutil.copy2(meta_path, backup_path.with_suffix(backup_path.suffix + ".meta"))


def verify_paths(scene_path: Path, required_paths: Iterable[str]) -> Dict[str, bool]:
    scene = SceneData(scene_path)
    return {path: scene.find_transform_by_path(path) is not None for path in required_paths}


def verify_paths_in_memory(scene: SceneData, required_paths: Iterable[str]) -> Dict[str, bool]:
    return {path: scene.find_transform_by_path(path) is not None for path in required_paths}


def main() -> int:
    parser = argparse.ArgumentParser(description="离线版 Unity scene 局部同步工具（当前仅支持纯场景对象子树）。")
    parser.add_argument("--source", required=True, help="源场景路径")
    parser.add_argument("--target", required=True, help="目标场景路径")
    parser.add_argument("--mode", choices=("copy-missing", "overwrite"), default="copy-missing")
    parser.add_argument("--path", dest="paths", action="append", required=True, help="要同步的对象路径，可重复传入")
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
        "mode": args.mode,
        "paths": args.paths,
        "dry_run": args.dry_run,
        "results": [],
    }

    try:
        if args.backup and not args.dry_run:
            write_backup(target_path, Path(args.backup))
            report["backup"] = args.backup

        for path in args.paths:
            result = clone_subtree_safe(source, target, path, args.mode)
            cast_results = report["results"]
            assert isinstance(cast_results, list)
            cast_results.append(result)

        serialized = target.serialize()
        if not args.dry_run:
            target_path.write_text(serialized, encoding="utf-8")

        report["verification"] = (
            verify_paths_in_memory(target, args.paths)
            if args.dry_run
            else verify_paths(target_path, args.paths)
        )
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
