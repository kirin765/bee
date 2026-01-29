# bee — Unity project

간단한 안내: 이 리포지토리를 GitHub에 올리려면 아래 사항을 권장합니다.

- 필수: Git LFS 설치 (대용량 에셋 관리)

  ```powershell
  # Windows (관리자 권한 권장)
  git lfs install
  git lfs track "*.png" "*.jpg" "*.psd" "*.fbx" "*.wav" "*.mp4"
  git add .gitattributes
  ```

- 기본 Git 초기화 및 푸시 예시

  ```powershell
  cd C:\Users\kwan7\bee
  git init
  git add .
  git commit -m "Initial commit"
  git remote add origin https://github.com/<your-org-or-user>/bee.git
  git push -u origin main
  ```

- 주의사항
  - `Library/`, `Temp/`, `obj/` 등은 `.gitignore`에 포함되어 있으므로 커밋하지 마세요.
  - 대형 바이너리 에셋(텍스처, 오디오, 모델)은 Git LFS로 관리하세요. LFS가 없으면 푸시가 실패하거나 리포지토리가 커집니다.
  - Unity Editor 버전: [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt) — `6000.3.2f1` 사용 권장.

- CI(선택): GitHub Actions로 Unity 빌드/테스트 자동화를 구성할 수 있습니다. 예시 워크플로우는 `.github/workflows/unity-ci.yml`에 포함되어 있습니다. Unity 라이선스 및 LFS 관련 시크릿(예: `UNITY_LICENSE`, `UNITY_EMAIL`) 설정이 필요합니다.

더 도와드릴까요? (예: GitHub 리포지토리 생성 및 첫 푸시 대신 터미널에서 직접 수행해드릴 수 있습니다.)
