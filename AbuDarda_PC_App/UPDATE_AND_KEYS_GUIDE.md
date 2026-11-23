# Abu Darda v4.2 - Developer Keys & Update Registry

## 1. User Data & Storage Architecture (PC Software)
The PC App stores all user-generated data (tasks, habits, vault logs, flashcards) in a dedicated directory:
* **Storage Location:** `d:\Abu Darda V 4.2\AbuDarda_PC_App\app_data\user_profile\`
* **Backups Folder:** `d:\Abu Darda V 4.2\AbuDarda_PC_App\app_data\user_data\backups\`
* **Exports Folder:** `d:\Abu Darda V 4.2\AbuDarda_PC_App\app_data\user_data\exports\`

> **Note:** Updating the app code or HTML will **never erase** user tasks or data because the code (`app_data\index.html`) is separated from the storage engine (`app_data\user_profile\`).

---

## 2. Future Updates & Cryptographic Signing Keys

### Android APK Release Signing Key:
* **Key File:** `d:\Abu Darda V 4.2\release.keystore`
* **Key Alias:** `abudardakey`
* **Key Password / Store Password:** `abudarda2026`
* **Validity:** 10,000 Days (~27 Years)
* **SHA-256 Digest:** `217a7b9961eeb8f100f5cb75edfe7c85b814e87b51bd98008dedf3138653c723`

### How to apply future updates:
1. **PC App Update:** Run `Update_App.bat` inside `AbuDarda_PC_App`. It automatically copies your latest `index.html` updates into the app while preserving all user databases.
2. **Android APK Update:** Compile the new HTML into the APK container using the `release.keystore` key so users can install it over the previous version without losing their progress.
