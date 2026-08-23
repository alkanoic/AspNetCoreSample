#!/usr/bin/env bash
# セッションごとに git worktree を作成し、その中で opencode を起動するラッパー。
# 使い方:
#   ./worktree-session.sh                       # 日時ベースの名前で worktree を作成し TUI を起動
#   ./worktree-session.sh issue-123             # 任意の名前で worktree を作成し TUI を起動
#   ./worktree-session.sh --mode server foo     # worktree のみ作成（opencode サーバー/web 運用向け）
#   ./worktree-session.sh --mode none foo       # worktree のみ作成（起動しない）
#
# 生成物:
#   - worktree ディレクトリ: ../<name>/
#   - ブランチ: feat/<name>（後で PR 経由の merge に乗せやすい）
#
# --mode の値:
#   tui     (既定) worktree 内で opencode の TUI を起動する
#   server  worktree のみ作成し、パスを表示する（web UI で開く運用向け）
#   none    worktree のみ作成する
#
# 既存 worktree と衝突する場合は何もせず終了する。
set -euo pipefail

mode="tui"
args=()
while [[ $# -gt 0 ]]; do
  case "$1" in
    --mode)
      mode="${2:?--mode には値を指定してください}"
      shift 2
      ;;
    --mode=*)
      mode="${1#--mode=}"
      shift
      ;;
    --help|-h)
      sed -n '2,12p' "$0" | sed 's/^# \{0,1\}//'
      exit 0
      ;;
    -*)
      echo "不明なオプション: $1" >&2
      exit 2
      ;;
    *)
      args+=("$1")
      shift
      ;;
  esac
done

case "$mode" in
  tui|server|none) ;;
  *)
    echo "--mode は tui / server / none のいずれかを指定してください" >&2
    exit 2
    ;;
esac

name="${args[0]:-session-$(date +%Y%m%d-%H%M%S)}"
branch="feat/${name}"
dir="../${name}"

# リポジトリのルートで実行する（git worktree add はルートからの相対を扱いやすい）
repo_root="$(git rev-parse --show-toplevel)"
cd "${repo_root}"

# 既存 worktree と衝突しないか確認
if git worktree list --porcelain | grep -q "worktree ${repo_root}/${name}$" \
  || git worktree list --porcelain | grep -q "worktree ${dir}$"; then
  echo "既に存在します: ${dir}" >&2
  exit 1
fi

git worktree add "${dir}" -b "${branch}"
echo "worktree 作成: ${dir} (branch: ${branch})"

case "$mode" in
  tui)
    (cd "${dir}" && opencode)
    ;;
  server)
    echo "opencode サーバーで開くディレクトリ: ${repo_root}/${name}"
    ;;
  none)
    ;;
esac
