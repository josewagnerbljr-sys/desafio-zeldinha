# 🗡️ Desafio Zeldinha – Violet's Awakening Expanded

**Dungeon Crawler 3D** inspirado em *The Legend of Zelda: Link's Awakening* e *Tunic*, criado como projeto final do **Módulo 6 – Zeldinha** da Trilha Unity da Digital Innovation One (DIO).

**Autor:** José Wagner Blanco Junior  
**Repositório:** https://github.com/josewagnerbljr-sys/desafio-zeldinha  
**Base original:** [digitalinnovationone/trilha-unity – Módulo 6 Zeldinha](https://github.com/digitalinnovationone/trilha-unity)

---

## 📖 História Expandida

Violet desperta em uma ilha misteriosa envolta em névoa roxa.  
Há séculos o **Lich Lord Malakar** selou um artefato proibido chamado **Coração da Aurora**.  
Com o selo se enfraquecendo, a corrupção se espalha e monstros antigos despertam.

Sua missão:
1. Explorar as masmorras da ilha
2. Coletar os **3 Fragmentos de Luz**
3. Derrotar o Lich Lord Malakar em sua forma final
4. Restaurar a luz e libertar a ilha

Cada Fragmento de Luz concede um poder permanente (mais vida, mais stamina ou golpe especial).

---

## 🎮 Controles

| Ação              | Teclado          | Mouse          |
|-------------------|------------------|----------------|
| Mover             | WASD             | -              |
| Pular             | Espaço           | -              |
| Ataque (combo)    | -                | Botão Esquerdo |
| Escudo / Parry    | -                | Botão Direito  |
| Interagir / Usar  | E                | -              |
| Inventário        | I ou Tab         | -              |
| Usar Poção Vida   | 1                | -              |
| Usar Poção Stamina| 2                | -              |
| Pausa             | Esc              | -              |

---

## ✨ Melhorias Implementadas neste Desafio

### Jogabilidade
- Sistema de **combo de 3 golpes** com timing
- **Stamina** para ataques e corrida
- **Parry** perfeito com o escudo (janela curta de i-frames + contra-ataque)
- Knockback e hit-stop para feedback satisfatório
- Câmera top-down suave com bounds

### Monstros
- **Melee Creature** melhorado (estados Idle → Follow → Attack → Hurt → Dead)
- **Ranged Creature** (novo) – atira projéteis
- **Lich Boss** com 3 fases reais:
  - Fase 1: ataques normais + teleporte
  - Fase 2: ritual de invocação + super ataque
  - Fase 3: forma enfurecida + invocações constantes
- Drops de itens e Fragmentos de Luz

### Itens & Inventário
- Sistema de inventário simples
- Poções de Vida e Stamina
- Fragmentos de Luz (objetivos principais)
- Chaves e baús
- Equipamentos com bônus de status

### Sistemas
- GameManager centralizado
- Sistema de Save/Load (PlayerPrefs + JSON básico)
- Diálogos e cutscenes leves
- UI de vida, stamina, inventário e objetivos
- Audio Manager

---

## 🛠️ Como instalar e testar

### Opção A – Usar junto com o projeto original da DIO (recomendado)

1. Baixe o projeto original da DIO:
   ```bash
   git clone https://github.com/digitalinnovationone/trilha-unity.git
   ```
2. Baixe os assets oficiais (link no README original do Zeldinha).
3. Abra o projeto **Módulo 6 - Zeldinha/Zeldinha** no Unity 2021.3 LTS ou superior.
4. Substitua / adicione os scripts desta pasta `Assets/Scripts` nos locais correspondentes.
5. Crie ou ajuste as cenas usando os novos scripts.

### Opção B – Projeto limpo (este ZIP)

1. Extraia este ZIP.
2. Abra a pasta no Unity Hub (versão recomendada: **2021.3 LTS** ou **2022.3 LTS**).
3. Instale os assets listados abaixo pela Asset Store (os mesmos do projeto original).
4. Configure a cena principal `Assets/Scenes/MainDungeon.unity` (criar manualmente ou copiar do original).
5. Pressione Play.

### Assets necessários (mesmos do projeto original da DIO)

- RPG Tiny Hero Duo PBR Polyart (Dungeon Mason)
- RPG Monster Duo / Partners PBR Polyart
- Mini Legion Rock Golem / Lich / Grunt
- 3D Dungeon Lowpoly Pack (CatBorg Studio)
- Polygonal's Low-Poly Particle Pack
- RPG Essentials Sound Effects
- Hit VFX Mega Pack
- Sprites Kenney + músicas Kevin MacLeod

Link de download em lote (SharePoint da DIO):  
https://academiapme-my.sharepoint.com/:f:/g/personal/leonardo_ropelato_dio_me/EpRQt5ZHIVJHi0ChnSaiVzgBPXL1csMIUGzv6_8sDAJJgQ

---

## 📁 Estrutura do Projeto

```
desafio-zeldinha/
├── Assets/
│   ├── Scripts/
│   │   ├── Player/          → PlayerController, PlayerCombat, PlayerStats, Inventory
│   │   ├── Enemies/         → MeleeEnemy, RangedEnemy, EnemyBase
│   │   ├── Boss/            → LichBossController + States
│   │   ├── Items/           → Item, Consumable, Key, LightFragment
│   │   ├── Systems/         → GameManager, SaveSystem, AudioManager
│   │   ├── Story/           → DialogueSystem, QuestManager
│   │   └── UI/              → HUD, InventoryUI, DialogueUI
│   ├── Scenes/
│   ├── Prefabs/
│   ├── ScriptableObjects/
│   └── Resources/
├── ProjectSettings/
├── Packages/
├── .gitignore
└── README.md
```

---

## 🚀 Passo a passo para subir no GitHub (Git Bash)

```bash
# 1. Entre na pasta do projeto
cd caminho/para/desafio-zeldinha

# 2. Inicialize (se ainda não for repositório)
git init

# 3. Adicione o remote (já existe)
git remote add origin https://github.com/josewagnerbljr-sys/desafio-zeldinha.git

# 4. Adicione tudo
git add .

# 5. Commit
git commit -m "feat: projeto completo Desafio Zeldinha - história, combate, monstros e itens melhorados"

# 6. Push
git branch -M main
git push -u origin main
```

---

## 📌 Observações importantes

- Este ZIP contém **todos os scripts C# melhorados** prontos para uso.
- Prefabs, cenas e modelos 3D devem ser importados dos assets oficiais ou do projeto original da DIO.
- Após importar, corrija eventuais referências no Inspector.
- O código foi escrito de forma limpa e comentada para facilitar correções futuras.

---

## 📜 Licença e Créditos

- Código original base: Rafa Skoberg + DIO
- Expansão e melhorias: José Wagner Blanco Junior
- Assets: respectivos criadores (Dungeon Mason, CatBorg, Kenney, Kevin MacLeod, etc.)

---

**Per Aspera, Ad Astra** ✨

Feito com ❤️ para o Desafio de Projeto da Trilha Unity – DIO.
