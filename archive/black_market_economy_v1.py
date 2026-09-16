import pygame
import random
import logging
from enum import Enum
import time

# Initialize Logging
logging.basicConfig(level=logging.INFO, filename="game_log.txt")

# Initialize Pygame
logging.info("Starting Pygame")
pygame.init()

# Constants
MAP_WIDTH, MAP_HEIGHT = 2000, 2000
BLACK, WHITE, YELLOW, RED, GRAY, BLUE, GREEN = (0, 0, 0), (255, 255, 255), (255, 255, 0), (255, 0, 0), (128, 128, 128), (0, 0, 255), (0, 255, 0)
GRID_CELL_SIZE = 100
NPC_COUNT, POLICE_COUNT = 20, 5
MAX_LIVES = 3
FIAT_INFLATION_INTERVAL = 60000  # 60 seconds
PROPERTY_INCOME_LEGAL = 30
PROPERTY_INCOME_ILLEGAL = 50
PROPERTY_MAINTENANCE_COST = 10
HURRICANE_CHANCE = 0.002
HURRICANE_DURATION = 300000  # 5 minutes
AGORIST_BODYGUARD_COST = 1500
AGORIST_BODYGUARD_DURATION = 300000  # 5 minutes
DETECTION_CHANCE_TAX = 0.15
DETECTION_CHANCE_BUSINESS = 0.02
ELECTION_INTERVAL = 1200000  # 20 minutes
POLICE_SALARY = 50  # Per police per minute
CAR_SPEED = 10  # Speed for cars
MIN_GAME_TIME_FOR_WIN = 300000  # 5 minutes in milliseconds

# SEK3 Quote and Free Roger Ver Text
SEK3_QUOTE = [
    "The basic principle which leads a libertarian from statism to his free society is the",
    "same which the founders of libertarianism used to discover the theory itself. That",
    "principle is consistency. Thus, the consistent application of the theory of",
    "libertarianism to every action the individual libertarian takes creates the libertarian",
    "society."
]
FREE_ROGER_VER = "Free Roger Ver"

# Enums
class Faction(Enum):
    NEUTRAL = "neutral"
    AGORIST = "agorists"
    STATIST = "statist"

class PropertyUse(Enum):
    LEGAL_BUSINESS = "LEGAL_BUSINESS"
    ILLEGAL_BUSINESS = "ILLEGAL_BUSINESS"
    MUTUAL_AID = "MUTUAL_AID"

# Spatial Grid for Efficient NPC Management
class SpatialGrid:
    def __init__(self, cell_size):
        self.cell_size = cell_size
        self.grid = {}

    def add_sprite(self, sprite):
        key = (sprite.rect.x // self.cell_size, sprite.rect.y // self.cell_size)
        if key not in self.grid:
            self.grid[key] = []
        self.grid[key].append(sprite)

    def update_sprite(self, sprite, old_rect):
        old_key = (old_rect.x // self.cell_size, old_rect.y // self.cell_size)
        new_key = (sprite.rect.x // self.cell_size, sprite.rect.y // self.cell_size)
        if old_key != new_key:
            if old_key in self.grid and sprite in self.grid[old_key]:
                self.grid[old_key].remove(sprite)
            self.add_sprite(sprite)

    def get_nearby(self, sprite):
        key = (sprite.rect.x // self.cell_size, sprite.rect.y // self.cell_size)
        nearby = []
        for dx in [-1, 0, 1]:
            for dy in [-1, 0, 1]:
                check_key = (key[0] + dx, key[1] + dy)
                if check_key in self.grid:
                    nearby.extend(self.grid[check_key])
        return nearby

# Car Class
class Car(pygame.sprite.Sprite):
    def __init__(self, x, y):
        super().__init__()
        self.image = pygame.Surface((64, 32))
        self.image.fill(GRAY)
        self.rect = self.image.get_rect(center=(x, y))

# Special NPC Class with Name
class SpecialNPC(pygame.sprite.Sprite):
    def __init__(self, x, y, name):
        super().__init__()
        self.image = pygame.Surface((32, 32))
        self.image.fill(GREEN)
        self.rect = self.image.get_rect(center=(x, y))
        self.is_agorist = True
        self.fiat_balance = 200
        self.name = name

    def update(self, game_state, clock_time):
        pass

# Assassin NPC Class
class NPCAssassin(pygame.sprite.Sprite):
    def __init__(self, x, y, target_id, orderer_id=None):
        super().__init__()
        self.image = pygame.Surface((32, 32))
        self.image.fill(RED)
        self.rect = self.image.get_rect(center=(x, y))
        self.target_id = target_id
        self.orderer_id = orderer_id
        self.shoot_timer = 0
        self.speed = 3

    def update(self, game_state, clock_time):
        self.shoot_timer += clock_time
        if self.shoot_timer >= 1000:
            self.shoot_timer = 0
            if self.target_id == "player":
                if self.rect.colliderect(game_state.player.rect):
                    game_state.player.health -= 50
                    logging.info("Assassin hit player, health -50!")
                    return True
            else:
                for politician in game_state.current_politicians:
                    if politician["id"] == self.target_id:
                        politician["reputation"] -= 20
                        logging.info(f"Assassin hit politician {self.target_id}, reputation -20!")
                        return True
        return False

# Player Class
class Player(pygame.sprite.Sprite):
    def __init__(self, x, y):
        super().__init__()
        self.image = pygame.Surface((32, 32))
        self.image.fill(WHITE)
        self.rect = self.image.get_rect(center=(x, y))
        self.health = 100
        self.speed = 5
        self.in_car = False
        self.car = None
        self.use_cooldown = 0

    def update(self, game_state, clock_time):
        keys = pygame.key.get_pressed()
        if keys[pygame.K_t]:
            game_state.toggle_tax_evasion()
        if keys[pygame.K_e]:
            if not self.in_car:
                for car in game_state.cars:
                    if self.rect.colliderect(car.rect):
                        self.in_car = True
                        self.car = car
                        self.image.set_alpha(0)  # Hide player
                        break
            else:
                self.in_car = False
                self.car = None
                self.image.set_alpha(255)  # Show player
        if keys[pygame.K_f] and self.use_cooldown <= 0:
            game_state.use_product('food')
        self.use_cooldown = max(0, self.use_cooldown - clock_time)

        if not self.in_car:
            dx, dy = 0, 0
            if keys[pygame.K_w]: dy -= 1
            if keys[pygame.K_s]: dy += 1
            if keys[pygame.K_a]: dx -= 1
            if keys[pygame.K_d]: dx += 1
            if dx != 0 or dy != 0:
                dist = (dx ** 2 + dy ** 2) ** 0.5
                self.rect.x += self.speed * dx / dist
                self.rect.y += self.speed * dy / dist
            self.rect.clamp_ip(pygame.Rect(0, 0, MAP_WIDTH, MAP_HEIGHT))
        else:
            dx, dy = 0, 0
            if keys[pygame.K_w]: dy -= 1
            if keys[pygame.K_s]: dy += 1
            if keys[pygame.K_a]: dx -= 1
            if keys[pygame.K_d]: dx += 1
            if dx != 0 or dy != 0:
                dist = (dx ** 2 + dy ** 2) ** 0.5
                self.car.rect.x += CAR_SPEED * dx / dist
                self.car.rect.y += CAR_SPEED * dy / dist
                self.car.rect.clamp_ip(pygame.Rect(0, 0, MAP_WIDTH, MAP_HEIGHT))
                self.rect.center = self.car.rect.center

# NPC Trader Class
class NPCTrader(pygame.sprite.Sprite):
    def __init__(self, x, y):
        super().__init__()
        self.image = pygame.Surface((32, 32))
        self.image.fill(YELLOW)
        self.rect = self.image.get_rect(center=(x, y))
        self.fiat_balance = 100
        self.is_agorist = random.random() < 0.2
        self.is_statist = random.random() < 0.2 and not self.is_agorist
        self.evading_taxes = self.is_agorist
        self.wanted_stars = 0

    def update(self, game_state, clock_time):
        if self.fiat_balance < 75:
            self.wealth_level = "poor"
        elif self.fiat_balance < 150:
            self.wealth_level = "middle"
        else:
            self.wealth_level = "rich"

# NPC Police Class
class NPCPolice(pygame.sprite.Sprite):
    def __init__(self, x, y):
        super().__init__()
        self.image = pygame.Surface((32, 32))
        self.image.fill(BLUE)
        self.rect = self.image.get_rect(center=(x, y))

# Property Class
class Property(pygame.sprite.Sprite):
    def __init__(self, x, y, use, owner_id):
        super().__init__()
        self.image = pygame.Surface((48, 48))
        self.image.fill(BLUE if use == PropertyUse.LEGAL_BUSINESS else RED if use == PropertyUse.ILLEGAL_BUSINESS else GRAY)
        self.rect = self.image.get_rect(center=(x, y))
        self.use = use
        self.owner_id = owner_id
        self.fiat_balance = 0
        self.dirty_fiat = 0

    def update(self, game_state, clock_time):
        if self.owner_id == "player":
            income_per_minute = PROPERTY_INCOME_ILLEGAL if self.use == PropertyUse.ILLEGAL_BUSINESS else PROPERTY_INCOME_LEGAL
            income_per_minute -= PROPERTY_MAINTENANCE_COST
            if game_state.economy.hurricane_active:
                income_per_minute = int(income_per_minute * 1.5)
            income = (income_per_minute / 60000) * clock_time
            if game_state.evading_taxes:
                self.dirty_fiat += income
            else:
                game_state.fiat_balance += income

# Economy Manager
class EconomyManager:
    def __init__(self):
        self.fiat_supply = 10000
        self.crypto_supply = 1000
        self.tax_revenue = 0
        self.state_spending = 0
        self.income_tax_rate = 0.15
        self.sales_tax_rate = 0.07
        self.crime_rate = 0.0
        self.agorist_score = 0
        self.statist_score = 0
        self.inflation_timer = 0
        self.hurricane_active = False
        self.hurricane_timer = 0
        self.election_timer = 0
        self.ubi_active = False
        self.food_stamps_active = False
        self.tariffs_active = False
        self.mass_deportation_active = False
        self.police_crackdown_active = False
        self.war_active = False
        self.cannabis_legal = False
        self.cocaine_legal = False
        self.heroin_legal = False
        self.all_drugs_illegal = False
        self.relaxed_border_policy = False
        self.strict_border_policy = False
        self.cctv_active = False
        self.relief_funding_active = False
        self.stimulus_check_active = False
        self.incarcerate_bitcoin_jesus_active = False
        self.game_start_time = None

    def update(self, game_state, clock_time):
        self.inflation_timer += clock_time
        self.election_timer += clock_time
        if self.inflation_timer >= FIAT_INFLATION_INTERVAL:
            self.inflation_timer = 0
            self.fiat_supply *= 1.001
        if not self.hurricane_active and random.random() < HURRICANE_CHANCE:
            self.hurricane_active = True
            self.hurricane_timer = HURRICANE_DURATION
            logging.info("Hurricane event started!")
        if self.hurricane_active:
            self.hurricane_timer -= clock_time
            if self.hurricane_timer <= 0:
                self.hurricane_active = False
                logging.info("Hurricane event ended!")
        if self.election_timer >= ELECTION_INTERVAL:
            self.election_timer = 0
            self.run_election(game_state)
        self.update_market_scores(game_state, clock_time)
        if self.game_start_time is not None and pygame.time.get_ticks() - self.game_start_time > MIN_GAME_TIME_FOR_WIN:
            winner = self.check_win_condition()
            if winner:
                game_state.running = False
                logging.info(f"Game Over: {winner.capitalize()} Win!")
        if self.ubi_active:
            for npc in game_state.npc_traders:
                npc.fiat_balance += 8 * (clock_time / 60000)
            self.state_spending += 2000 * (clock_time / 60000)
            self.crime_rate = min(10, self.crime_rate + 0.15 * (clock_time / 60000))
        if self.food_stamps_active:
            for npc in game_state.npc_traders:
                if npc.wealth_level == "poor":
                    npc.fiat_balance += 5 * (clock_time / 60000)
            self.state_spending += 500 * (clock_time / 60000)
            self.crime_rate = min(10, self.crime_rate + 0.05 * (clock_time / 60000))
        if self.tariffs_active:
            self.tax_revenue += 500 * (clock_time / 60000)
            self.sales_tax_rate = 0.12
        if self.mass_deportation_active:
            self.crime_rate = min(10, self.crime_rate + 0.1 * (clock_time / 60000))
        if self.police_crackdown_active:
            game_state.wanted_stars = min(5, game_state.wanted_stars + 0.05 * (clock_time / 60000))
        if self.war_active:
            self.crime_rate = min(10, self.crime_rate + 0.3 * (clock_time / 60000))
        if self.cannabis_legal or self.cocaine_legal or self.heroin_legal:
            self.crime_rate = max(0, self.crime_rate - 0.05 * (clock_time / 60000))
        if self.all_drugs_illegal:
            self.crime_rate = min(10, self.crime_rate + 0.2 * (clock_time / 60000))
        if self.relaxed_border_policy:
            self.tax_revenue -= 50 * (clock_time / 60000)
        if self.strict_border_policy:
            self.tax_revenue += 50 * (clock_time / 60000)
        if self.cctv_active:
            game_state.wanted_stars = min(5, game_state.wanted_stars + 0.02 * (clock_time / 60000))
        if self.relief_funding_active and self.hurricane_active:
            for politician in game_state.current_politicians:
                politician["reputation"] += 5 * (clock_time / 60000)
        if self.stimulus_check_active:
            for npc in game_state.npc_traders:
                npc.fiat_balance += 100
            self.state_spending += 1000
            self.stimulus_check_active = False
        if self.incarcerate_bitcoin_jesus_active:
            self.agorist_score -= 50 * (clock_time / 60000)
            self.crypto_supply -= 100 * (clock_time / 60000)

    def update_market_scores(self, game_state, clock_time):
        agorist_increment = (game_state.crypto_balance * 0.01 + sum(n.fiat_balance for n in game_state.npc_traders if n.is_agorist) * 0.001) * (clock_time / 1000)
        self.agorist_score += agorist_increment
        statist_increment = (game_state.fiat_balance * 0.01 + len(game_state.police) * 0.1) * (clock_time / 1000)
        self.statist_score += statist_increment

    def check_win_condition(self):
        total_score = self.agorist_score + self.statist_score
        if total_score > 0:
            agorist_pct = (self.agorist_score / total_score) * 100
            statist_pct = (self.statist_score / total_score) * 100
            if agorist_pct >= 90:
                return "agorists"
            elif statist_pct >= 90:
                return "statist"
        return None

    def run_election(self, game_state):
        legislations = [
            "Enable UBI", "Enable Food Stamps", "Enable Tariffs", "Mass Deportation", 
            "Police Crackdown", "War", "Legalize Cannabis", "Legalize Cocaine", 
            "Legalize Heroin", "All Drugs Illegal", "Relaxed Border Policy", 
            "Strict Border Policy", "CCTV Legislation", "Relief Funding",
            "Stimulus Check Legislation", "Incarcerate Bitcoin Jesus"
        ]
        options = random.sample(legislations, 2)
        votes = {options[0]: 0, options[1]: 0}
        for politician in game_state.current_politicians:
            choice = random.choice(options)
            votes[choice] += 1
            politician["reputation"] += random.randint(5, 15)
            politician["terms_served"] += 1
        winner = max(votes, key=votes.get)
        logging.info(f"Election held! Legislation '{winner}' won with {votes[winner]} votes!")
        if winner == "Enable UBI":
            self.ubi_active = True
        elif winner == "Enable Food Stamps":
            self.food_stamps_active = True
        elif winner == "Enable Tariffs":
            self.tariffs_active = True
        elif winner == "Mass Deportation":
            self.mass_deportation_active = True
        elif winner == "Police Crackdown":
            self.police_crackdown_active = True
        elif winner == "War":
            self.war_active = True
        elif winner == "Legalize Cannabis":
            self.cannabis_legal = True
        elif winner == "Legalize Cocaine":
            self.cocaine_legal = True
        elif winner == "Legalize Heroin":
            self.heroin_legal = True
        elif winner == "All Drugs Illegal":
            self.all_drugs_illegal = True
        elif winner == "Relaxed Border Policy":
            self.relaxed_border_policy = True
        elif winner == "Strict Border Policy":
            self.strict_border_policy = True
        elif winner == "CCTV Legislation":
            self.cctv_active = True
        elif winner == "Relief Funding":
            self.relief_funding_active = True
        elif winner == "Stimulus Check Legislation":
            self.stimulus_check_active = True
        elif winner == "Incarcerate Bitcoin Jesus":
            self.incarcerate_bitcoin_jesus_active = True

# Game State
class GameState:
    def __init__(self):
        self.economy = EconomyManager()
        self.fiat_balance = 500
        self.crypto_balance = 500
        self.player_faction = Faction.NEUTRAL
        self.wanted_stars = 0
        self.all_sprites = pygame.sprite.Group()
        self.npcs = SpatialGrid(GRID_CELL_SIZE)
        self.properties = []
        self.npc_traders = []
        self.police = []
        self.cars = []  # Added for cars
        self.special_npcs = []
        self.assassins = []
        self.lives = MAX_LIVES
        self.player = None
        self.camera_x = 0
        self.camera_y = 0
        self.evading_taxes = False
        self.running = True
        self.resources = {"food": 10}  # Inventory
        self.inventory_open = False
        self.special_npcs_unlocked = {"Brandon Aragon": False, "Sal Mayweather": False}
        self.current_politicians = [{"id": i, "reputation": 0, "pay": 100, "terms_served": 0} for i in range(3)]

    def initialize_game(self):
        self.player = Player(MAP_WIDTH // 2, MAP_HEIGHT // 2)
        self.all_sprites.add(self.player)
        self.npcs.add_sprite(self.player)
        for _ in range(NPC_COUNT):
            trader = NPCTrader(random.randint(0, MAP_WIDTH), random.randint(0, MAP_HEIGHT))
            self.npc_traders.append(trader)
            self.all_sprites.add(trader)
            self.npcs.add_sprite(trader)
        for _ in range(POLICE_COUNT):
            police = NPCPolice(random.randint(0, MAP_WIDTH), random.randint(0, MAP_HEIGHT))
            self.police.append(police)
            self.all_sprites.add(police)
            self.npcs.add_sprite(police)
        for _ in range(5):
            car = Car(random.randint(0, MAP_WIDTH), random.randint(0, MAP_HEIGHT))
            self.cars.append(car)
            self.all_sprites.add(car)
        property = Property(random.randint(0, MAP_WIDTH), random.randint(0, MAP_HEIGHT), PropertyUse.ILLEGAL_BUSINESS, "player")
        self.properties.append(property)
        self.all_sprites.add(property)
        self.economy.game_start_time = pygame.time.get_ticks()

    def toggle_tax_evasion(self):
        self.evading_taxes = not self.evading_taxes
        logging.info(f"Tax evasion {'enabled' if self.evading_taxes else 'disabled'}")

    def use_product(self, product):
        if product in self.resources and self.resources[product] > 0 and self.player.use_cooldown <= 0:
            if product == 'food':
                self.resources['food'] -= 1
                self.player.health = min(100, self.player.health + 20)
                self.player.use_cooldown = 60000
                logging.info("Used food, health +20")

    def update(self, clock_time):
        self.economy.update(self, clock_time)
        for sprite in self.all_sprites:
            old_rect = sprite.rect.copy()
            sprite.update(self, clock_time)
            self.npcs.update_sprite(sprite, old_rect)
        self.unlock_special_npc()
        if random.random() < 0.001:
            target = random.choice(["player"] + [p["id"] for p in self.current_politicians])
            assassin = NPCAssassin(random.randint(0, MAP_WIDTH), random.randint(0, MAP_HEIGHT), target)
            self.assassins.append(assassin)
            self.all_sprites.add(assassin)
            logging.info(f"Assassin spawned targeting {target}!")
        for assassin in self.assassins[:]:
            if assassin.update(self, clock_time):
                self.assassins.remove(assassin)
                self.all_sprites.remove(assassin)
        if self.player:
            self.camera_x = max(0, min(self.player.rect.x - SCREEN_WIDTH // 2, MAP_WIDTH - SCREEN_WIDTH))
            self.camera_y = max(0, min(self.player.rect.y - SCREEN_HEIGHT // 2, MAP_HEIGHT - SCREEN_HEIGHT))

# Main Menu and Game Loop
logging.info("Setting display mode")
screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))  # Windowed mode for stability
pygame.display.set_caption("Black Market Economy")
clock = pygame.time.Clock()
font = pygame.font.Font(None, 36)
game_state = GameState()

# Main Menu
in_menu = True
menu_options = ["N - New Game", "L - Load Game", "Q - Quit"]

while in_menu:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            pygame.quit()
            exit()
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_n:
                game_state.initialize_game()
                in_menu = False
            elif event.key == pygame.K_l:
                logging.info("Load Game selected (not implemented)")
            elif event.key == pygame.K_q:
                pygame.quit()
                exit()

    screen.fill(BLACK)
    title_text = font.render("Black Market Economy", True, WHITE)
    screen.blit(title_text, (SCREEN_WIDTH // 2 - title_text.get_width() // 2, SCREEN_HEIGHT // 4))
    for i, option in enumerate(menu_options):
        option_text = font.render(option, True, WHITE)
        screen.blit(option_text, (SCREEN_WIDTH // 2 - option_text.get_width() // 2, SCREEN_HEIGHT // 2 + i * 50))
    # Render SEK3 quote
    quote_y = SCREEN_HEIGHT // 2 + 150
    for line in SEK3_QUOTE:
        quote_text = font.render(line, True, WHITE)
        screen.blit(quote_text, (SCREEN_WIDTH // 2 - quote_text.get_width() // 2, quote_y))
        quote_y += 20
    # Render Free Roger Ver
    free_text = font.render(FREE_ROGER_VER, True, WHITE)
    screen.blit(free_text, (SCREEN_WIDTH - free_text.get_width() - 10, SCREEN_HEIGHT - free_text.get_height() - 10))
    pygame.display.flip()

# Game Loop
logging.info("Entering main loop")
while game_state.running:
    logging.info("Starting frame")
    clock_time = clock.tick(60)
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            game_state.running = False
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_i:
                game_state.inventory_open = not game_state.inventory_open

    game_state.update(clock_time)
    screen.fill(BLACK)
    for sprite in game_state.all_sprites:
        screen.blit(sprite.image, (sprite.rect.x - game_state.camera_x, sprite.rect.y - game_state.camera_y))
    
    for special_npc in game_state.special_npcs:
        dx = special_npc.rect.x - game_state.player.rect.x
        dy = special_npc.rect.y - game_state.player.rect.y
        distance = (dx ** 2 + dy ** 2) ** 0.5
        if distance < 100:
            name_text = font.render(special_npc.name, True, WHITE)
            screen.blit(name_text, (special_npc.rect.x - game_state.camera_x, special_npc.rect.y - game_state.camera_y - 20))
    
    if game_state.inventory_open:
        inventory_text = font.render("Inventory:", True, WHITE)
        screen.blit(inventory_text, (10, 100))
        for i, (item, count) in enumerate(game_state.resources.items()):
            item_text = font.render(f"{item}: {count}", True, WHITE)
            screen.blit(item_text, (10, 140 + i * 40))
    
    stats_text = font.render(f"Health: {game_state.player.health} Fiat: {game_state.fiat_balance} Crypto: {game_state.crypto_balance} Wanted: {game_state.wanted_stars}", True, WHITE)
    scores_text = font.render(f"Agorist: {game_state.economy.agorist_score:.1f} Statist: {game_state.economy.statist_score:.1f}", True, WHITE)
    screen.blit(stats_text, (10, 10))
    screen.blit(scores_text, (10, 50))
    pygame.display.flip()
    logging.info("Frame finished")

pygame.quit()
