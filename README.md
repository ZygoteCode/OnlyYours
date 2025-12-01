# OnlyYours Password Manager

[English](#english) | [Italiano](#italiano)

---

## English

OnlyYours is a secure, local password manager built with C# and .NET Framework 4.8.1. It provides robust encryption for storing and managing your sensitive credentials with military-grade security algorithms.

### Technical Architecture

#### Core Components
- **PasswordManager.cs**: Central class handling credential storage and cryptographic operations
- **Utils.cs**: Utility functions for hashing, compression, and data manipulation
- **MainForm.cs**: Primary UI implementing the MetroFramework design
- **MultipurposeForm.cs**: Dynamic form handling various user operations

#### Cryptographic Implementation

OnlyYours implements a multi-layered security approach:

1. **Password Hashing**:
   - Uses KECCAK-256 (SHA-3 family) for password hashing
   - Generates a 32-byte hash from user password for encryption key derivation

2. **Data Encryption**:
   - Implements AES-256 encryption via the SimpleCrypto library
   - Uses CBC mode with PKCS7 padding
   - Generates unique IV for each encryption operation

3. **Data Integrity**:
   - Employs KECCAK-512 for data hashing and integrity verification
   - Implements multiple checksum validations throughout the decryption process

4. **Storage Format**:
   ```
   [HEADER] + [FILE_HASH] + [COMPRESSED_DATA]
   
   COMPRESSED_DATA:
   [PASSWORD_PORTION] + [ENCRYPTION_HASH] + [ENCRYPTED_PAYLOAD]
   
   ENCRYPTED_PAYLOAD:
   [JSON_HASH] + [LENGTH_PREFIX] + [ENCRYPTED_JSON]
   ```

5. **Security Features**:
   - Zero-knowledge architecture (passwords never leave your device)
   - In-memory encryption/decryption (data never stored unencrypted)
   - Multi-stage validation with descriptive error handling
   - Secure random IV generation for each save operation

#### Libraries and Dependencies

- **Bouncy Castle Crypto**: Cryptographic primitives (KECCAK hashing)
- **SimpleCrypto**: AES encryption implementation
- **Newtonsoft.Json**: Credential serialization/deserialization
- **Guna UI2**: Modern UI controls and components
- **MetroSuite 2.0**: Windows Forms theme framework
- **Costura.Fody**: Embedded assembly management
- **Fody**: Compile-time code weaving

#### Data Flow

1. User enters master password
2. Password hashed with KECCAK-256 to derive encryption key
3. Credentials serialized to JSON format
4. JSON data hashed with KECCAK-512 for integrity
5. Length-prefixed and combined with hash
6. Entire payload encrypted with AES-256
7. Encrypted data compressed with GZIP
8. Final package prepended with file integrity hash
9. File saved with custom `.plf` extension

#### Security Validation Chain

During loading, OnlyYours performs multiple validation checks:
1. File integrity hash verification
2. Password-derived portion comparison
3. Encrypted payload hash validation
4. Decrypted JSON length verification
5. JSON content hash validation

Any mismatch results in immediate rejection with "Invalid password" error.

### System Requirements

- Windows 7 or higher
- .NET Framework 4.8.1
- 50MB free disk space

### Build Instructions

1. Open `OnlyYours.sln` in Visual Studio 2022 or newer
2. Restore NuGet packages: `nuget restore`
3. Build solution: `msbuild OnlyYours.sln /p:Configuration=Release`

---

## Italiano

OnlyYours è un gestore di password sicuro e locale costruito con C# e .NET Framework 4.8.1. Fornisce una crittografia robusta per archiviare e gestire le tue credenziali sensibili con algoritmi di sicurezza di livello militare.

### Architettura Tecnica

#### Componenti Principali
- **PasswordManager.cs**: Classe centrale che gestisce l'archiviazione delle credenziali e le operazioni crittografiche
- **Utils.cs**: Funzioni di utilità per hashing, compressione e manipolazione dei dati
- **MainForm.cs**: Interfaccia utente principale con design MetroFramework
- **MultipurposeForm.cs**: Form dinamico che gestisce varie operazioni dell'utente

#### Implementazione Crittografica

OnlyYours implementa un approccio di sicurezza multilivello:

1. **Hashing della Password**:
   - Utilizza KECCAK-256 (famiglia SHA-3) per l'hashing delle password
   - Genera un hash di 32 byte dalla password dell'utente per la derivazione della chiave di crittografia

2. **Crittografia dei Dati**:
   - Implementa la crittografia AES-256 tramite la libreria SimpleCrypto
   - Utilizza la modalità CBC con padding PKCS7
   - Genera un IV univoco per ogni operazione di crittografia

3. **Integrità dei Dati**:
   - Impiega KECCAK-512 per l'hashing e la verifica dell'integrità dei dati
   - Implementa molteplici convalide di checksum durante il processo di decrittografia

4. **Formato di Archiviazione**:
   ```
   [HEADER] + [FILE_HASH] + [DATI_COMPRESSI]
   
   DATI_COMPRESSI:
   [PORZIONE_PASSWORD] + [HASH_CRITTOGRAFIA] + [PAYLOAD_CRITTOGRAFATO]
   
   PAYLOAD_CRITTOGRAFATO:
   [HASH_JSON] + [LUNGHEZZA_PREFISSO] + [JSON_CRITTOGRAFATO]
   ```

5. **Caratteristiche di Sicurezza**:
   - Architettura zero-knowledge (le password non escono mai dal tuo dispositivo)
   - Crittografia/decrittografia in memoria (i dati non vengono mai memorizzati in chiaro)
   - Convalida multistadio con gestione degli errori descrittiva
   - Generazione sicura di IV casuale per ogni operazione di salvataggio

#### Librerie e Dipendenze

- **Bouncy Castle Crypto**: Primitive crittografiche (hashing KECCAK)
- **SimpleCrypto**: Implementazione della crittografia AES
- **Newtonsoft.Json**: Serializzazione/deserializzazione delle credenziali
- **Guna UI2**: Controlli e componenti dell'interfaccia utente moderna
- **MetroSuite 2.0**: Framework per il tema di Windows Forms
- **Costura.Fody**: Gestione degli assembly incorporati
- **Fody**: Intreccio del codice in fase di compilazione

#### Flusso dei Dati

1. L'utente inserisce la password principale
2. La password viene hashata con KECCAK-256 per derivare la chiave di crittografia
3. Le credenziali vengono serializzate nel formato JSON
4. I dati JSON vengono hashati con KECCAK-512 per l'integrità
5. Lunghezza prefissata e combinata con l'hash
6. L'intero payload viene crittografato con AES-256
7. I dati crittografati vengono compressi con GZIP
8. Il pacchetto finale viene preceduto dall'hash di integrità del file
9. Il file viene salvato con estensione personalizzata `.plf`

#### Catena di Validazione della Sicurezza

Durante il caricamento, OnlyYours esegue controlli di convalida multipli:
1. Verifica dell'hash di integrità del file
2. Confronto della porzione derivata dalla password
3. Validazione dell'hash del payload crittografato
4. Verifica della lunghezza del JSON decrittografato
5. Validazione dell'hash del contenuto JSON

Qualsiasi discrepanza comporta il rifiuto immediato con errore "Password non valida".

### Requisiti di Sistema

- Windows 7 o superiore
- .NET Framework 4.8.1
- 50MB di spazio libero su disco

### Istruzioni di Compilazione
1. Aprire `OnlyYours.sln` in Visual Studio 2022 o versione successiva
2. Ripristinare i pacchetti NuGet: `nuget restore`
3. Compilare la soluzione: `msbuild OnlyYours.sln /p:Configuration=Release`
