import os
import re
import time
import winreg
import platform

import paramiko
import wmi
import subprocess
import psutil
import sys
import ctypes
import fnmatch
from datetime import datetime, timedelta
import requests
from collections import defaultdict
from colorama import init, Fore
from psycopg2 import *

init(autoreset=True)

STRATZ_API_KEY = "this your api key"

RANK_NAMES = {
    1: "Рекрут",
    2: "Страж",
    3: "Рыцарь",
    4: "Герой",
    5: "Легенда",
    6: "Властелин",
    7: "Дивайн",
    8: "Титан"
}
print(Fore.CYAN + f"Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn")
print(Fore.CYAN + f"Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn")
print(Fore.CYAN + f"Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn")
print(Fore.CYAN + f"Модификация программы: 2x.05.2025")

def find_steam_folder():
    print(Fore.CYAN + "=== Шаг 1: Поиск папки Steam ===")
    search_paths = []

    try:
        reg_key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"Software\Valve\Steam")
        steam_exe_path, _ = winreg.QueryValueEx(reg_key, "SteamExe")
        steam_folder_from_registry = os.path.dirname(steam_exe_path)
        if os.path.exists(steam_folder_from_registry):
            print(Fore.GREEN + f"Папка Steam найдена: {steam_folder_from_registry}")
            return steam_folder_from_registry
    except FileNotFoundError:
        print(Fore.YELLOW + "Путь к Steam не найден.")

    program_files = os.getenv("ProgramFiles")
    program_files_x86 = os.getenv("ProgramFiles(x86)")

    if program_files:
        search_paths.append(program_files)
    if program_files_x86:
        search_paths.append(program_files_x86)

    drives = [f"{d}:\\" for d in "ABCDEFGHIJKLMNOPQRSTUVWXYZ" if os.path.exists(f"{d}:\\")]
    search_paths.extend(drives)

    exclude_folders = ["Sandbox"]

    for path in search_paths:
        print(Fore.YELLOW + f"Поиск в директории: {path}")
        for root, dirs, _ in os.walk(path):
            dirs[:] = [d for d in dirs if d not in exclude_folders]

            if "Steam" in dirs:
                steam_folder = os.path.join(root, "Steam")
                if os.path.exists(os.path.join(steam_folder, "userdata")):
                    print(Fore.GREEN + f"Папка Steam найдена: {steam_folder}")
                    return steam_folder
            elif "steam" in dirs:
                steam_folder = os.path.join(root, "steam")
                if os.path.exists(os.path.join(steam_folder, "userdata")):
                    print(Fore.GREEN + f"Папка Steam найдена: {steam_folder}")
                    return steam_folder
            break

    print(Fore.RED + "Папка Steam не найдена.")
    return None



def check_userdata_folders(steam_folder):
    print(Fore.CYAN + "\n=== Шаг 2: Проверка папок userdata ===")
    userdata_path = os.path.join(steam_folder, "userdata")
    account_ids = []
    if os.path.exists(userdata_path):
        print(Fore.GREEN + "Папка юзеров найдена")
        for folder in os.listdir(userdata_path):
            folder_path = os.path.join(userdata_path, folder)
            if os.path.isdir(folder_path) and folder.isdigit():
                print(Fore.YELLOW + f"Найдена папка аккаунта: {folder}")
                if os.path.exists(os.path.join(folder_path, "570")):
                    print(Fore.GREEN + f"Файл найден")
                    account_ids.append(folder)
                else:
                    print(Fore.RED + "Файл отсутствует")
    else:
        print(Fore.RED + f"Папка юзеров не найдена: {userdata_path}")
    print(Fore.CYAN + f"Найдено аккаунтов: {len(account_ids)}")
    return account_ids

def parse_logs(steam_folder, account_ids):
    print(Fore.CYAN + "\n=== Шаг 3: Парсинг логов ===")
    logs_path = os.path.join(steam_folder, "logs")
    log_files = ["connection_log.txt", "librarysharing_log.txt", "workshop_log.txt"]
    account_info = {}

    if not os.path.exists(logs_path):
        print(Fore.RED + f"Папка логов не найдена: {logs_path}")
        return account_info

    print(Fore.GREEN + "Папка логов найдена")

    today = datetime.now()
    two_weeks_ago = today - timedelta(days=14)

    for log_file in log_files:
        log_file_path = os.path.join(logs_path, log_file)
        if os.path.exists(log_file_path):
            print(Fore.YELLOW + "Анализ файлов...")
            with open(log_file_path, "r", encoding="utf-8") as file:
                lines = file.readlines()
                for line in lines:
                    # Извлекаем дату из строки
                    date_match = re.search(r'\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]', line)
                    if date_match:
                        log_date_str = date_match.group(1)
                        log_date = datetime.strptime(log_date_str, "%Y-%m-%d %H:%M:%S")
                        # Проверяем, попадает ли дата в диапазон последних 2 недель
                        if two_weeks_ago <= log_date <= today:
                            if log_file == "connection_log.txt":
                                match = re.search(r'\[U:1:(\d+)\]', line)
                                if match:
                                    account_id = match.group(1)
                                    if account_id in account_ids:
                                        account_info.setdefault(account_id, []).append(("connection", log_date_str))
                            elif log_file == "librarysharing_log.txt":
                                match = re.search(r'(current user|token ownerID) (\d+)', line)
                                if match:
                                    account_id = match.group(2)
                                    if account_id in account_ids:
                                        account_info.setdefault(account_id, []).append(("librarysharing", log_date_str))
                            elif log_file == "workshop_log.txt":
                                match = re.search(r'\[U:1:(\d+)\]', line)
                                if match:
                                    account_id = match.group(1)
                                    if account_id in account_ids:
                                        account_info.setdefault(account_id, []).append(("workshop", log_date_str))
        else:
            print(Fore.RED + f"Файл {log_file} не найден.")
    return account_info

def find_dota_folder(steam_folder):
    print(Fore.CYAN + "\n=== Шаг 4: Поиск папки dota 2 beta ===")

    try:
        reg_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570")
        install_location, _ = winreg.QueryValueEx(reg_key, "InstallLocation")
        if os.path.exists(install_location):
            print(Fore.GREEN + f"Папка dota 2 beta найдена: {install_location}")
            return install_location
    except FileNotFoundError:
        print(Fore.RED + "Путь к dota 2 beta не найден.")

    common_path = os.path.join(steam_folder, "steamapps", "common")
    if os.path.exists(common_path):
        print(Fore.GREEN + f"Папка common найдена: {common_path}")
        dota_path = os.path.join(common_path, "dota 2 beta")
        if os.path.exists(dota_path):
            print(Fore.GREEN + f"Папка dota 2 beta найдена: {dota_path}")
            return dota_path
        else:
            print(Fore.RED + "Папка dota 2 beta не найдена в common.")

    library_folders = []
    library_file = os.path.join(steam_folder, "steamapps", "libraryfolders.vdf")
    if os.path.exists(library_file):
        with open(library_file, "r", encoding="utf-8") as file:
            for line in file:
                match = re.search(r'"path"\s+"(.+)"', line)
                if match:
                    library_folders.append(match.group(1))
    for library in library_folders:
        dota_path = os.path.join(library, "steamapps", "common", "dota 2 beta")
        if os.path.exists(dota_path):
            print(Fore.GREEN + f"Папка dota 2 beta найдена в SteamLibrary: {dota_path}")
            return dota_path

    print(Fore.RED + "Папка dota 2 beta не найдена.")
    return None



def extract_account_id(filename):
    if filename.startswith('cache_'):
        parts = filename.split('_')
        if len(parts) >= 2:
            return parts[1]
    
    elif filename.startswith('latest_conduct_'):
        parts = filename.split('.')
        if len(parts) >= 2:
            return parts[1]
    
    return filename

def check_dota_files(dota_path, account_ids):
    print(Fore.CYAN + "\n=== Шаг 5: Проверка файлов в папке dota 2 beta ===")
    cfg_path = os.path.join(dota_path, "game", "dota", "cfg")
    cache_path = os.path.join(dota_path, "game", "dota")
    valid_accounts = []

    if not os.path.exists(dota_path):
        print(Fore.RED + f"Папка dota 2 beta не найдена: {dota_path}")
        return valid_accounts

    print(Fore.GREEN + f"Папка dota 2 beta найдена: {dota_path}")
    
    file_details = []
    
    for account_id in account_ids:
        cache_files = [
            f"cache_{account_id}.soc",
            f"cache_{account_id}_1.soc"
        ]
        
        for cache_file in cache_files:
            file_path = os.path.join(cache_path, cache_file)
            if os.path.exists(file_path):
                creation_time = os.path.getctime(file_path)
                modified_time = os.path.getmtime(file_path)
                file_details.append((
                    account_id,
                    datetime.fromtimestamp(creation_time),
                    datetime.fromtimestamp(modified_time),
                    "cache"
                ))
                valid_accounts.append(account_id)

        conduct_files = [
            f"latest_conduct_{account_id}.txt",
            f"latest_conduct_1.{account_id}.txt"
        ]
        
        for conduct_file in conduct_files:
            file_path = os.path.join(cfg_path, conduct_file)
            if os.path.exists(file_path):
                creation_time = os.path.getctime(file_path)
                modified_time = os.path.getmtime(file_path)
                file_details.append((
                    account_id,
                    datetime.fromtimestamp(creation_time),
                    datetime.fromtimestamp(modified_time),
                    "conduct"
                ))
                valid_accounts.append(account_id)

    # print(Fore.CYAN + "\nИнформация о файлах:")
    # for account_id, creation_time, modified_time, file_type in file_details:
    #     print(Fore.YELLOW + 
    #           f"{account_id} создан: {creation_time}, последнее изменение: {modified_time}")
    
    return valid_accounts

def parse_config_vdf(steam_folder):
    print(Fore.CYAN + "\n=== Шаг 6: Парсинг файла конфига ===")
    config_path = os.path.join(steam_folder, "config", "config.vdf")
    accounts = {}

    if not os.path.exists(config_path):
        print(Fore.RED + f"Файл конфига не найден")
        return accounts

    print(Fore.GREEN + f"Файл конфига найден")

    with open(config_path, "r", encoding="utf-8") as file:
        content = file.read()

        accounts_start = content.find('"Accounts"')
        if accounts_start == -1:
            print(Fore.RED + "Блок 'Accounts' не найден в config.")
            return accounts

        accounts_block = content[accounts_start:].split('}{')
        for i, account_block in enumerate(accounts_block):
            if i % 2 == 1:
                accounts_start_in_block = account_block.find('{')
                accounts_end_in_block = account_block.find('}')
                if accounts_start_in_block == -1 or accounts_end_in_block == -1:
                    print(Fore.RED + "Не удалось найти конец блока 'Accounts' в извлеченном блоке.")
                    return accounts
                account_block = account_block[accounts_start_in_block:accounts_end_in_block + 1]

            account_matches = re.finditer(r'"([^"]+)"\s*{\s*"SteamID"\s*"(\d+)"', account_block)
            for match in account_matches:
                account_name = match.group(1)
                steam_id = match.group(2)
                accounts[account_name] = steam_id
                print(Fore.GREEN + f"Найден аккаунт: {account_name}, SteamID: {steam_id}")

    return accounts

# def check_registry():
    # print(Fore.CYAN + "\n=== Шаг 7: Проверка Windows ===")
    # accounts = {}
    # try:
    #     reg_key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, r"Software\Valve\Steam\Users")
    #     for i in range(winreg.QueryInfoKey(reg_key)[0]):
    #         account_name = winreg.EnumKey(reg_key, i)
    #         account_key = winreg.OpenKey(reg_key, account_name)
    #         steam_id, _ = winreg.QueryValueEx(account_key, "SteamID")
    #         accounts[account_name] = steam_id
    #         print(Fore.GREEN + f"Найден аккаунт в реестре: {account_name}, SteamID: {steam_id}")
    # except FileNotFoundError:
    #     print(Fore.RED + "Реестр Steam не найден.")
    # return accounts

def get_windows_installation_date():
    # print(Fore.CYAN + "\n=== Шаг 7: Проверка Windows ===")
    try:
        result = subprocess.run(['systeminfo'], stdout=subprocess.PIPE, text=True, check=True)
        output = result.stdout

        for line in output.splitlines():
            if "„ в  гбв ­®ўЄЁ:" in line:
                return line.split(":", 1)[1].strip()
    except Exception as e:
        print(Fore.RED + f"Ошибка при получении даты установки Windows: {e}")
    return "Не удалось получить дату установки"

def check_windows_version():
    try:
        version = platform.version()
        release = platform.release()
        # print(Fore.CYAN + f"\n=== Версия Windows ===")
        # print(Fore.GREEN + f"Версия Windows: {release} (Build {version})")
        return f"Версия Windows: {release} (Build {version})"
    except Exception as e:
        print(Fore.RED + f"Ошибка при получении версии Windows: {e}")
    return "Не удалось получить версию Windows"

def get_windows_installation_date_wmi():
    try:
        c = wmi.WMI()
        for os in c.Win32_OperatingSystem():
            if os.Caption == 'Windows':
                return os.InstallDate
        return "Не удалось получить дату установки"
    except Exception as e:
        print(Fore.RED + f"Ошибка при получении даты установки Windows через WMI: {e}")

def get_windows_installation_date_registry():
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Microsoft\Windows NT\CurrentVersion")
        install_date = winreg.QueryValueEx(key, "InstallDate")[0]
        return install_date
    except Exception as e:
        print(Fore.RED + f"Ошибка при получении даты установки Windows через реестр: {e}")
    return "Не удалось получить дату установки"

def get_windows_installation_date_powershell():
    try:
        result = subprocess.run(['powershell', '-Command', 'Get-ComputerInfo | Select-Object -Expand OsInstallDate'], stdout=subprocess.PIPE, text=True, check=True, shell=True)
        return result.stdout.strip()
    except Exception as e:
        print(Fore.RED + f"Ошибка при получении даты установки Windows через PowerShell: {e}")
    return "Не удалось получить дату установки"

def get_windows_install_date():
    try:
        command = "powershell -command \"Get-WmiObject -Class Win32_OperatingSystem | Select-Object -ExpandProperty InstallDate\""
        install_date = subprocess.check_output(command, shell=True).decode().strip()

        if install_date:
            install_date = install_date.split('.')[0]
            install_date = datetime.strptime(install_date, "%Y%m%d%H%M%S")
            formatted_date = install_date.strftime("%Y-%m-%d %H:%M:%S")
            
            current_date = datetime.now()
            days_since_install = (current_date - install_date).days

            if days_since_install < 30:
                return Fore.RED + install_date.strftime("%Y-%m-%d %H:%M:%S") + " | Windows была установлена менее 30 дней назад."
            else:
                return Fore.GREEN + install_date.strftime("%Y-%m-%d %H:%M:%S") + " | Windows была установлена более 30 дней назад."

        else:
            return "Не удалось получить дату установки"
    except Exception as e:
        return f"Ошибка: {e}"

def check_for_cheats():
    print(Fore.CYAN + "\n=== Шаг 8: Проверка на читы ===")
    user_profile = os.getenv('USERPROFILE')
    melonity_path = os.path.join(user_profile, "AppData", "Roaming", "Melonity")
    logs_path = os.path.join(melonity_path, "Logs")

    if not os.path.exists(melonity_path):
        print(Fore.GREEN + "Папка Melonity не найдена.")
        return

    if not os.path.exists(logs_path):
        print(Fore.RED + "Папка Logs в Melonity не найдена.")
        return

    print(Fore.GREEN + "Папка Logs в Melonity найдена.")

    latest_log_path = os.path.join(logs_path, "latest.log")
    if os.path.exists(latest_log_path):
        with open(latest_log_path, "r", encoding="utf-8") as file:
            lines = file.readlines()
            if lines:
                last_line = lines[-1].strip()
                print(Fore.YELLOW + f"Последняя строка в latest.log: {last_line}")

                date_match = re.search(r'(\d{4})-(\d{2})-(\d{2})', last_line)
                if date_match:
                    log_date = datetime.strptime(date_match.group(0), "%Y-%m-%d")
                    current_date = datetime.now().date()
                    if log_date.date() == current_date:
                        print(Fore.RED + "Последняя активность Melonity совпадает с текущей датой.")

    for file_name in os.listdir(logs_path):
        if file_name.startswith("melonity-") and file_name.endswith(".log"):
            file_date_str = file_name.split('-')[1]
            file_date = datetime.strptime(file_date_str, "%Y-%m-%d")
            if file_date.date() == datetime.now().date():
                file_path = os.path.join(logs_path, file_name)
                with open(file_path, "r", encoding="utf-8") as file:
                    for line in file:
                        if "[info] Initializing..." in line:
                            print(Fore.RED + f"Инициализация Melonity в {file_name}: {line.strip()}")
                            break
                        
def check_virtual_machine():
    print(Fore.CYAN + "\n=== Шаг 9: Проверка на виртуальную машину ===")
    vm_indicators = []
    
    try:
        c = wmi.WMI()
        for bios in c.Win32_BIOS():
            manufacturer = bios.Manufacturer.lower()
            if 'vmware' in manufacturer:
                vm_indicators.append("BIOS производителя VMware обнаружен")
            elif 'virtualbox' in manufacturer:
                vm_indicators.append("BIOS производителя VirtualBox обнаружен")
            elif 'qemu' in manufacturer:
                vm_indicators.append("BIOS производителя QEMU обнаружен")
            elif 'xen' in manufacturer:
                vm_indicators.append("BIOS производителя Xen обнаружен")
            elif 'innotek' in manufacturer:
                vm_indicators.append("BIOS производителя Innotek (VirtualBox) обнаружен")
            elif 'kvm' in manufacturer:
                vm_indicators.append("BIOS производителя KVM обнаружен")
                
        for compsys in c.Win32_ComputerSystem():
            manufacturer = compsys.Manufacturer.lower()
            model = compsys.Model.lower()
            
            if 'vmware' in manufacturer:
                vm_indicators.append(f"Производитель системы: VMware ({compsys.Manufacturer})")
            if 'virtualbox' in manufacturer:
                vm_indicators.append(f"Производитель системы: VirtualBox ({compsys.Manufacturer})")
            if 'qemu' in manufacturer:
                vm_indicators.append(f"Производитель системы: QEMU ({compsys.Manufacturer})")
                
            if 'vmware' in model:
                vm_indicators.append(f"Модель системы: VMware ({compsys.Model})")
            if 'virtualbox' in model:
                vm_indicators.append(f"Модель системы: VirtualBox ({compsys.Model})")
            if 'qemu' in model:
                vm_indicators.append(f"Модель системы: QEMU ({compsys.Model})")
            if 'virtual machine' in model:
                vm_indicators.append(f"Модель системы: Virtual Machine ({compsys.Model})")
    except Exception as e:
        vm_indicators.append(f"Ошибка при проверке WMI: {str(e)}")
    
    try:
        vm_registry_checks = [
            (r"HARDWARE\ACPI\DSDT", "VBOX__", "Обнаружен ключ реестра VirtualBox (ACPI DSDT)"),
            (r"HARDWARE\ACPI\FADT", "VBOX__", "Обнаружен ключ реестра VirtualBox (ACPI FADT)"),
            (r"HARDWARE\ACPI\RSDT", "VBOX__", "Обнаружен ключ реестра VirtualBox (ACPI RSDT)"),
            (r"SYSTEM\CurrentControlSet\Control\SystemInformation", "SystemProductName", 
             lambda v: "VMware" if "VMware" in v else ("VirtualBox" if "VirtualBox" in v else None)),
            (r"SYSTEM\CurrentControlSet\Control\SystemInformation", "SystemManufacturer", 
             lambda v: "VMware" if "VMware" in v else ("VirtualBox" if "VirtualBox" in v else None)),
            (r"SOFTWARE\Oracle\VirtualBox Guest Additions", None, "Обнаружены гостевые дополнения VirtualBox"),
            (r"SOFTWARE\VMware, Inc.\VMware Tools", None, "Обнаружены инструменты VMware")
        ]
        
        for key, value, message in vm_registry_checks:
            try:
                reg_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, key)
                if value is None:
                    vm_indicators.append(message)
                else:
                    try:
                        reg_value = winreg.QueryValueEx(reg_key, value)[0]
                        if callable(message):
                            result = message(reg_value)
                            if result:
                                vm_indicators.append(f"{result} обнаружен в реестре ({key}\\{value})")
                        elif value in reg_value:
                            vm_indicators.append(f"{message} ({reg_value})")
                    except WindowsError:
                        pass
            except WindowsError:
                pass
    except Exception as e:
        vm_indicators.append(f"Ошибка при проверке реестра: {str(e)}")
    
    try:
        result = subprocess.run(['systeminfo'], stdout=subprocess.PIPE, text=True, check=True)
        output = result.stdout.lower()
        
        if 'vmware' in output:
            vm_indicators.append("Systeminfo показывает признаки VMware")
        if 'virtualbox' in output:
            vm_indicators.append("Systeminfo показывает признаки VirtualBox")
        if 'hyper-v' in output:
            vm_indicators.append("Systeminfo показывает признаки Hyper-V")
        if 'qemu' in output:
            vm_indicators.append("Systeminfo показывает признаки QEMU")
        if 'xen' in output:
            vm_indicators.append("Systeminfo показывает признаки Xen")
    except Exception as e:
        vm_indicators.append(f"Ошибка при выполнении systeminfo: {str(e)}")
    
    vm_files = [
        (r"C:\Windows\System32\Drivers\VBoxGuest.sys", "Драйвер VirtualBox обнаружен"),
        (r"C:\Windows\System32\Drivers\vm3dgl.sys", "Драйвер VMware 3D Graphics обнаружен"),
        (r"C:\Windows\System32\Drivers\vmmouse.sys", "Драйвер мыши VMware обнаружен"),
        (r"C:\Windows\System32\Drivers\vmhgfs.sys", "Драйвер файловой системы VMware обнаружен"),
        (r"C:\Windows\System32\Drivers\vmusbmouse.sys", "Драйвер USB мыши VMware обнаружен"),
        (r"C:\Windows\System32\Drivers\vmx_svga.sys", "Драйвер SVGA VMware обнаружен"),
        (r"C:\Windows\System32\Drivers\vmxnet.sys", "Драйвер сети VMware обнаружен"),
        (r"C:\Windows\System32\Drivers\VMToolsHook.dll", "DLL инструментов VMware обнаружена"),
        (r"C:\Windows\System32\Drivers\vmhgfs.dll", "DLL файловой системы VMware обнаружена")
    ]
    
    for file, message in vm_files:
        if os.path.exists(file):
            vm_indicators.append(message)
    
    try:
        c = wmi.WMI()
        vm_processes = [
            ("vmtoolsd.exe", "Процесс VMware Tools запущен"),
            ("vmwaretray.exe", "Процесс VMware Tray запущен"),
            ("vmwareuser.exe", "Процесс VMware User запущен"),
            ("vboxservice.exe", "Сервис VirtualBox запущен"),
            ("vboxtray.exe", "Процесс VirtualBox Tray запущен"),
            ("xenservice.exe", "Сервис Xen запущен"),
            ("qemu-ga.exe", "QEMU Guest Agent запущен")
        ]
        
        for process in c.Win32_Process():
            for proc_name, message in vm_processes:
                if process.Name.lower() == proc_name.lower():
                    vm_indicators.append(message)
    except Exception as e:
        vm_indicators.append(f"Ошибка при проверке процессов: {str(e)}")

    try:
        for nic in c.Win32_NetworkAdapterConfiguration():
            if nic.MACAddress:
                mac = nic.MACAddress.lower()
                vm_mac_prefixes = {
                    "00:05:69": "MAC VMware (00:05:69)",
                    "00:0c:29": "MAC VMware (00:0C:29)",
                    "00:1c:14": "MAC VMware (00:1C:14)",
                    "00:50:56": "MAC VMware (00:50:56)",
                    "08:00:27": "MAC VirtualBox (08:00:27)",
                    "00:16:3e": "MAC Xen (00:16:3E)",
                    "00:1a:4a": "MAC KVM (00:1A:4A)",
                    "00:15:5d": "MAC Hyper-V (00:15:5D)"
                }
                for prefix, message in vm_mac_prefixes.items():
                    if mac.startswith(prefix.lower()):
                        vm_indicators.append(message)
    except Exception as e:
        vm_indicators.append(f"Ошибка при проверке MAC-адресов: {str(e)}")
    
    if vm_indicators:
        print(Fore.RED + "\nОбнаружены следующие признаки виртуальной машины:")
        for indicator in sorted(set(vm_indicators)):
            print(Fore.YELLOW + f"  • {indicator}")
        
        vm_types = {
            "VMware": sum(1 for i in vm_indicators if "VMware" in i),
            "VirtualBox": sum(1 for i in vm_indicators if "VirtualBox" in i),
            "Hyper-V": sum(1 for i in vm_indicators if "Hyper-V" in i),
            "QEMU": sum(1 for i in vm_indicators if "QEMU" in i),
            "Xen": sum(1 for i in vm_indicators if "Xen" in i),
            "KVM": sum(1 for i in vm_indicators if "KVM" in i)
        }
        
        probable_vm = max(vm_types.items(), key=lambda x: x[1])
        if probable_vm[1] > 0:
            print(Fore.RED + f"\nВероятная виртуальная машина: {probable_vm[0]} (найдено {probable_vm[1]} признаков)")
        else:
            print(Fore.RED + "\nОбнаружены признаки виртуальной машины, но тип определить не удалось")
        
        return True, vm_indicators
    else:
        print(Fore.GREEN + "Признаков виртуальной машины не обнаружено")
        return False, []

def get_current_exe():
    """Получаем путь к текущему исполняемому файлу"""
    try:
        return os.path.abspath(psutil.Process().exe())
    except:
        return None

def fast_delete(file_path):
    """Быстрое удаление файла с минимальными проверками"""
    try:
        try:
            os.remove(file_path)
            if not os.path.exists(file_path):
                return True
        except:
            pass
        
        try:
            ctypes.windll.kernel32.SetFileAttributesW(file_path, 0x80)
            os.unlink(file_path)
            return not os.path.exists(file_path)
        except:
            return False
    except:
        return False

def find_and_remove_enrage_files():
    """Основная функция поиска и удаления файлов"""
    current_exe = get_current_exe()
    if not current_exe:
        print(Fore.RED + "Не удалось определить текущий EXE-файл!")
        return [], []

    search_paths = [
        os.path.join(os.environ['USERPROFILE'], 'Downloads'),
        os.path.join(os.environ['USERPROFILE'], 'Desktop'),
        os.path.join(os.environ['USERPROFILE'], 'Documents'),
        os.path.join(os.environ['USERPROFILE'], 'AppData', 'Local', 'Temp')
    ]

    patterns = [
        "enrage_checker*.exe",
        "enrage checker*.exe",
        "enragechecker*.exe",
        "enrage-checker*.exe"
    ]

    deleted_files = []
    failed_files = []

    for folder in search_paths:
        if not os.path.exists(folder):
            continue

        for root, _, files in os.walk(folder):
            for pattern in patterns:
                for filename in fnmatch.filter(files, pattern):
                    full_path = os.path.abspath(os.path.join(root, filename))
                    
                    if full_path.lower() == current_exe.lower():
                        continue
                    
                    if fast_delete(full_path):
                        deleted_files.append(full_path)
                    else:
                        failed_files.append(full_path)

    return deleted_files, failed_files


def get_account_rank(account_id):
    try:
        url = "https://api.stratz.com/graphql"
        payload = {
            "query": """
            {
              player(steamAccountId: %s) {
                ranks {
                  rank
                }
                steamAccount {
                  seasonLeaderboardRank
                }
              }
            }
            """ % account_id,
            "variables": {}
        }
        headers = {
            'Authorization': f'Bearer {STRATZ_API_KEY}',
            'User-Agent': 'STRATZ_API',
            'Content-Type': 'application/json'
        }
        
        response = requests.post(url, headers=headers, json=payload, timeout=10)
        
        if response.status_code != 200:
            print(Fore.YELLOW + f"Ошибка запроса к STRATZ API: статус {response.status_code}")
            return "Unknown (API Error)"
            
        data = response.json()
        
        if "errors" in data:
            print(Fore.YELLOW + f"Ошибка в ответе STRATZ: {data['errors']}")
            return "Unknown (API Error)"
            
        ranks = data.get("data", {}).get("player", {}).get("ranks", [])
        if ranks:
            rank = ranks[0].get("rank", "Unknown")
            if rank != "Unknown":
                try:
                    rank_number = int(str(rank)[0])
                    stars = int(str(rank)[1])
                    rank_name = RANK_NAMES.get(rank_number, "Неизвестный ранг")
                    return f"{rank_name} ({stars} звезд)"
                except (ValueError, IndexError):
                    return "Unknown (Invalid Rank Format)"
                    
        return "Unknown (No Rank Data)"
        
    except requests.exceptions.RequestException as e:
        print(Fore.YELLOW + f"Ошибка подключения к STRATZ API: {str(e)}")
        return "Unknown (Connection Error)"
    except Exception as e:
        print(Fore.YELLOW + f"Неожиданная ошибка при запросе к STRATZ API: {str(e)}")
        return "Unknown (Error)"


# def get_account_games(account_id):
#     url = 'https://api.stratz.com/graphql'
#     payload = {
#         "query": """
#         {
#           player(steamAccountId: %s) {
#               matches(request: { take:50 })
#               { """ % account_id +
#                  """
#                      players(steamAccountId: %s)
#                      {
#                          match{
#                              isStats
#                          }
#                          hero{
#                              displayName
#                          }
#                          kills
#                          deaths
#                          isVictory       
#                      }
#                    }
#                }
#              }
#              """ % account_id,
#         "variables": {}
#     }
#     headers = {
#         'Authorization': f'Bearer {STRATZ_API_KEY}',
#         'User-Agent': 'STRATZ_API',
#         'Content-Type': 'application/json'
#     }
#     response = requests.post(url, json=payload, headers=headers)
#     if response.status_code == 200:
#         data = response.json()
#         try:
#             matches = data['data']['player']['matches']
#             total_kills = 0
#             total_deaths = 0
#             wins = 0
#             losses = 0
#             hero_games = defaultdict(int)
#             sleaver_suggest = False
#             sleave_count = 0
#             nonsleave_count = 0
#             games_count = ""
#             data = []

#             # Обрабатываем матчи
#             for match in matches:
#                 players = match['players']
#                 for player in players:
#                     is_win = player['isVictory']
#                     if is_win:
#                         wins += 1
#                     else:
#                         losses += 1

#                 total_kills += player.get('kills', 0)
#                 total_deaths += player.get('deaths', 0)

#                 hero_name = player['hero']['displayName']
#                 hero_games[hero_name] += 1
#                 is_stats = player['match']['isStats']
#                 if is_stats:
#                     sleave_count += 1
#                 else:
#                     nonsleave_count += 1

#             if(nonsleave_count > 0):
#                 if (sleave_count // nonsleave_count) > 1:
#                     sleaver_suggest = True

#             total_matches = wins + losses
#             average_kills = total_kills / total_matches if total_matches > 0 else 0
#             average_deaths = total_deaths / total_matches if total_matches > 0 else 0

#             total_stats = f"Количество побед: {wins}\n" + f"Количество поражений: {losses}\n" + f"Среднее количество убийств: {average_kills:.2f}\n" + f"Среднее количество смертей: {average_deaths:.2f}\n\n" + "Количество игр на персонажах:\n"

#             for hero, count in hero_games.items():
#                 games_count = f"{hero}: {count} игр\n"
#                 data.append(games_count)

#             games_count = sorted(data, key=extract_games_count, reverse=True)
#             total_stats += ("".join(games_count) + f"\n\nПодозрение на сливерскую деятельность: {sleaver_suggest}\n")

#             return account_id, total_stats
#         except Exception as e:
#             print(e)
#             return ("Unknown")
#     else:
#         return ("Unknown")


def extract_games_count(entry):
    return int(entry.split(':')[1].strip().split()[0])

def check_key_exists(user_input):
    try:
        # connect to your database
        conn = connect(
            host="",
            port="",
            database="",
            user="",
            password=""
        )    
        
        cur = conn.cursor()

        query = "SELECT EXISTS(Select key from program_keys where key=%s);"
        cur.execute(query, (user_input,))

        exists = cur.fetchone()[0] 

        cur.close()
        conn.close()

        return exists

    except Exception as e:
        print("Ошибка при работе с базой данных:", e)
        return False

#connect to your vps from ssh
SSH_HOST = ''
SSH_USER = ''
SSH_PASS = ''
REMOTE_VERSION_FILE = '/root/EnrageChecker/version.txt'
REMOTE_EXE_FILE = '/root/EnrageChecker/Enrage Checker.exe'

LOCAL_VERSION = '1.0.7'



def get_local_version():
    return LOCAL_VERSION

def get_remote_version(ssh):
    sftp = ssh.open_sftp()
    try:
        with sftp.open(REMOTE_VERSION_FILE, 'r') as f:
            version = f.read()
            if isinstance(version, bytes):
                version = version.decode('utf-8')
            version = version.strip()
        return version
    finally:
        sftp.close()

def download_new_exe(ssh, local_path):
    sftp = ssh.open_sftp()
    try:
        sftp.get(REMOTE_EXE_FILE, local_path)
    finally:
        sftp.close()

def version_greater(v1, v2):
    def parse(v):
        return tuple(map(int, (v.split("."))))
    return parse(v1) > parse(v2)

def create_and_run_update_bat(current_exe_path, new_exe_path):
    bat_path = os.path.join(os.path.dirname(current_exe_path), "update.bat")
    exe_name = os.path.basename(current_exe_path)
    new_name = os.path.basename(new_exe_path)

    bat_content = f"""@echo off
echo Ожидаем завершения программы {exe_name}...
:waitloop
tasklist /fi "imagename eq {exe_name}" | find /i "{exe_name}" >nul
if not errorlevel 1 (
    timeout /t 1 /nobreak >nul
    goto waitloop
)
echo Удаляем старую версию...
del "{current_exe_path}"
echo Переименовываем новую версию...
rename "{new_exe_path}" "{exe_name}"
echo Запускаем обновленную программу...
start "" "{current_exe_path}"
echo Удаляем этот скрипт...
del "%~f0"
"""

    with open(bat_path, "w", encoding="utf-8") as f:
        f.write(bat_content)

    # Запускаем батник и завершаем текущий процесс
    subprocess.Popen([bat_path], shell=True)
    print("Запущен обновляющий скрипт, завершаем текущий процесс.")
    sys.exit()

def upload_local_exe_and_version(ssh, local_exe_path, local_version):
    sftp = ssh.open_sftp()
    try:
        # Загружаем локальный exe на сервер (перезаписываем)
        sftp.put(local_exe_path, REMOTE_EXE_FILE)
        print(f"Загружен локальный exe на сервер: {REMOTE_EXE_FILE}")

        # Записываем локальную версию в version.txt
        with sftp.open(REMOTE_VERSION_FILE, 'w') as f:
            f.write(local_version)
        print(f"Обновлен файл версии на сервере: {REMOTE_VERSION_FILE} с версией {local_version}")
    finally:
        sftp.close()

def main():
    print("Запуск проверки обновлений...")

    current_exe = sys.executable
    local_version = get_local_version()

    ssh = paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())

    try:
        ssh.connect(SSH_HOST,22, SSH_USER, SSH_PASS)
        remote_version = get_remote_version(ssh)
        print(f"Локальная версия: {local_version}")
        print(f"Версия на сервере: {remote_version}")

        if version_greater(remote_version, local_version):
            print("Доступна новая версия! Скачиваем...")
            new_exe_path = current_exe + ".new"
            download_new_exe(ssh, new_exe_path)
            print(f"Скачано новое обновление: {new_exe_path}")
            create_and_run_update_bat(current_exe, new_exe_path)

        elif version_greater(local_version, remote_version):
            print("Локальная версия новее удалённой! Загружаем на сервер...")
            upload_local_exe_and_version(ssh, current_exe, local_version)
            print("Обновление сервера завершено.")

        else:
            print("У вас установлена последняя версия.")

    except Exception as e:
        print("Ошибка при обновлении:", e)

    finally:
        ssh.close()

    is_correct_key = False

    while (is_correct_key == False):
        user_value = input("Введите ключ активации: ")
        if check_key_exists(user_value):
            print("Ключ активации корректный")
            is_correct_key = True
        else:
            # очистка консоли
            if os.name == 'nt':
                os.system('cls')
            # для Linux и MacOS
            else:
                os.system('clear')
            print("Неверный ключ активации, повторите попытку: ")

    today = datetime.now()
    report_filename = f"{today.date()}_{today.time().strftime('%H-%M')}- ENRAGE проверка.txt"
    report_folder = os.path.join(os.path.expanduser("~"), "Documents", "Enrage")
    report_filepath = os.path.join(report_folder, report_filename)  # Full path to the report file
    directory = os.path.dirname(report_filepath)
    if not os.path.exists(directory):
        os.makedirs(directory)

    steam_folder = find_steam_folder()
    if not steam_folder:
        return

    account_ids = check_userdata_folders(steam_folder)
    account_info = parse_logs(steam_folder, account_ids)
    dota_path = find_dota_folder(steam_folder)
    
    if dota_path:
        valid_accounts = check_dota_files(dota_path, account_ids)
    else:
        valid_accounts = []       

    config_accounts = parse_config_vdf(steam_folder)
    # registry_accounts = check_registry()

    found_accounts = set(valid_accounts)
    not_found_accounts = set(account_ids) - found_accounts
    accounts_with_logs = set()
    accounts_with_rank = set()
    accounts_without_logs = set()
    accounts_unknown_rank = set()
    games_info = defaultdict()
    sleaver_suggest = False

    for account_id in account_ids:
        if account_id in account_info:
            accounts_with_logs.add(account_id)
        else:
            accounts_without_logs.add(account_id)

    for account_id in account_ids:
        rank = get_account_rank(account_id)
        if rank != "Unknown":
            accounts_with_rank.add(account_id)
        else:
            accounts_unknown_rank.add(account_id)

    accounts_with_rank_no_logs = accounts_with_rank - accounts_with_logs

    config_unique_accounts = {}
    for account_name, steam_id in config_accounts.items():
        if steam_id not in account_ids:
            config_unique_accounts[account_name] = steam_id

    # Аккаунты + результаты матчей за последние 50 матчей
    # for account_id in account_ids:
    #     values = get_account_games(account_id)
    #     if values != "Unknown":
    #         steam_id, games_data = values
    #         games_info[steam_id] = games_data

    print(Fore.CYAN + "\n=== Итоговый результат ===")
    print(Fore.CYAN + "1. Найденные аккаунты:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn\n")
        report_file.write("Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn\n")
        report_file.write("Программа создана для проекта Enrage - Турниры для маленьких рангов DOTA 2 от: @nevercr7 и @vladimirrogozn\n")
        report_file.write("\n1. Найденные аккаунты:\n")
        
        for account_id in found_accounts:
            print(Fore.GREEN + f"  {account_id}")
            report_file.write(f"    {account_id}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n2. Не найденные аккаунты:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n2. Не найденные аккаунты:\n")
        for account_id in not_found_accounts:
            print(Fore.RED + f"  {account_id}")
            report_file.write(f"    {account_id}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n3. Аккаунты с логами:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n3. Аккаунты с логами:\n")
        for account_id in accounts_with_logs:
            if account_id in account_info:
                last_log = account_info[account_id][-1]  # Последний лог
                print(Fore.GREEN + f"  Аккаунт ID: {account_id}, Последний лог: {last_log[1]}")
                rank = get_account_rank(account_id)
                print(Fore.RED + f"    Ранг: {rank}")
                report_file.write(f"    Аккаунт ID: {account_id}, Последний лог: {last_log[1]}\n")
                report_file.write(f"    Ранг: {rank}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n4. Аккаунты с рангом, но без логов:\n")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n4. Аккаунты с рангом, но без логов:\n")
        for account_id in accounts_with_rank_no_logs:
            rank = get_account_rank(account_id)
            print(Fore.YELLOW + f"  Аккаунт ID: {account_id}, Ранг: {rank}")
            report_file.write(f"    Аккаунт ID: {account_id}, Ранг: {rank}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n5. Аккаунты с неизвестным рангом:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n5. Аккаунты с неизвестным рангом:\n")
        for account_id in accounts_unknown_rank:
            print(Fore.RED + f"  Аккаунт ID: {account_id}, Ранг: Unknown")
            report_file.write(f"    Аккаунт ID: {account_id}, Ранг: Unknown\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n6. Аккаунты из конфигов:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n6. Аккаунты из конфигов:\n")
        for account_name, steam_id in config_accounts.items():
            print(Fore.YELLOW + f"  {account_name} - {steam_id}")
            report_file.write(f"    {account_name} - {steam_id}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    # print(Fore.CYAN + "\n7. Аккаунты из реестра:")
    # try:
    #     report_file = open(report_filepath, "a", encoding="utf-8")
    #     report_file.write("\n7. Аккаунты из реестра:\n")
    #     for account_name, steam_id in registry_accounts.items():
    #         print(Fore.GREEN + f"  {account_name}: {steam_id}")
    #         report_file.write(f"    {account_name}: {steam_id}\n")
    #     report_file.close()
    # except Exception as e:
    #     print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n7. Изменения файлов:")
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n7. Изменения файлов:\n")
        
        cfg_path = os.path.join(dota_path, "game", "dota", "cfg")
        cache_path = os.path.join(dota_path, "game", "dota")
        
        cache_files = []
        conduct_files = []

        for account_id in account_ids:
            cache_file = os.path.join(cache_path, f"cache_{account_id}.soc")
            cache_file_1 = os.path.join(cache_path, f"cache_{account_id}_1.soc")
            
            if os.path.exists(cache_file):
                creation_time = os.path.getctime(cache_file)
                modified_time = os.path.getmtime(cache_file)
                cache_files.append((account_id, creation_time, modified_time))
                
            if os.path.exists(cache_file_1):
                creation_time = os.path.getctime(cache_file_1)
                modified_time = os.path.getmtime(cache_file_1)
                cache_files.append((account_id, creation_time, modified_time))

        for account_id in account_ids:
            conduct_file = os.path.join(cfg_path, f"latest_conduct_{account_id}.txt")
            conduct_file_1 = os.path.join(cfg_path, f"latest_conduct_1.{account_id}.txt")
            
            if os.path.exists(conduct_file):
                creation_time = os.path.getctime(conduct_file)
                modified_time = os.path.getmtime(conduct_file)
                conduct_files.append((account_id, creation_time, modified_time))
                
            if os.path.exists(conduct_file_1):
                creation_time = os.path.getctime(conduct_file_1)
                modified_time = os.path.getmtime(conduct_file_1)
                conduct_files.append((account_id, creation_time, modified_time))

        if cache_files:
            print(Fore.MAGENTA + "\nКеш файлы:")
            report_file.write("\nКеш файлы:\n")
            for account_id, creation_time, modified_time in cache_files:
                line = f"{account_id} создан: {datetime.fromtimestamp(creation_time)}, последнее изменение: {datetime.fromtimestamp(modified_time)}"
                print(Fore.YELLOW + f"  {line}")
                report_file.write(f"  {line}\n")

        if conduct_files:
            print(Fore.MAGENTA + "\nКондукт файлы:")
            report_file.write("\nКондукт файлы:\n")
            for account_id, creation_time, modified_time in conduct_files:
                line = f"{account_id} создан: {datetime.fromtimestamp(creation_time)}, последнее изменение: {datetime.fromtimestamp(modified_time)}"
                print(Fore.YELLOW + f"  {line}")
                report_file.write(f"  {line}\n")

        if os.path.exists(cache_path) and os.path.exists(cfg_path):
            cache_folder_time = datetime.fromtimestamp(os.path.getctime(cache_path))
            conduct_folder_time = datetime.fromtimestamp(os.path.getctime(cfg_path))
            
            print(Fore.YELLOW + f"\nПапка кеш создана: {cache_folder_time}")
            print(Fore.YELLOW + f"Папка кондукт создана: {conduct_folder_time}")
            
            report_file.write(f"\nПапка кеш создана: {cache_folder_time}\n")
            report_file.write(f"Папка кондукт создана: {conduct_folder_time}\n")
        
        if cache_files:
            earliest_cache_file = min(cache_files, key=lambda x: x[1])
            earliest_cache_file_time = datetime.fromtimestamp(earliest_cache_file[1])
            cache_folder_creation_time_dt = cache_folder_time

            days_difference_cache = (earliest_cache_file_time - cache_folder_creation_time_dt).days
            if days_difference_cache > 1:
                print(Fore.RED + f"Обнаружена возможная чистка файлов в папке кеш: разница между созданием папки и самого раннего файла более {days_difference_cache} дней.")
                report_file.write(f"Обнаружена возможная чистка файлов в папке кеш: разница между созданием папки и самого раннего файла более {days_difference_cache} дней.\n")

        if conduct_files:
            earliest_conduct_file = min(conduct_files, key=lambda x: x[1])
            earliest_conduct_file_time = datetime.fromtimestamp(earliest_conduct_file[1])
            conduct_folder_creation_time_dt = conduct_folder_time

            days_difference_conduct = (earliest_conduct_file_time - conduct_folder_creation_time_dt).days
            if days_difference_conduct > 1:
                print(Fore.RED + f"Обнаружена возможная чистка файлов в папке кондукт: разница между созданием папки и самого раннего файла более {days_difference_conduct} дней.")
                report_file.write(f"Обнаружена возможная чистка файлов в папке кондукт: разница между созданием папки и самого раннего файла более {days_difference_conduct} дней.\n")

        if conduct_files:
            earliest_conduct_file = min(conduct_files, key=lambda x: x[1])
            earliest_conduct_file_time = datetime.fromtimestamp(earliest_conduct_file[1])
            conduct_folder_creation_time_dt = conduct_folder_time

            if abs(earliest_conduct_file_time - conduct_folder_creation_time_dt) > timedelta(days=1):
                print(Fore.RED + "Обнаружена возможная чистка файлов в папке кондукт: разница между созданием папки и самого раннего файла более 1 дня.")
                report_file.write("Обнаружена возможная чистка файлов в папке кондукт: разница между созданием папки и самого раннего файла более 1 дня.\n")


        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    print(Fore.CYAN + "\n8. Информация о windows:")
    windows_version_info = check_windows_version()
    windows_installation_date = get_windows_installation_date()
    # windows_installation_date_registry = get_windows_installation_date_registry()
    # windows_installation_date_wmi = get_windows_installation_date_wmi()
    # windows_installation_date_powershell = get_windows_installation_date_powershell()
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        print(Fore.CYAN + "\n=== Версия Windows ===\n")
        print(Fore.YELLOW + f"{windows_version_info}\n")
        # print(Fore.YELLOW + f"Дата установки Windows: {windows_installation_date}\n")
        # print(Fore.YELLOW + f"Дата установки Windows реестр: {windows_installation_date_registry}\n")
        # print(Fore.YELLOW + f"Дата установки Windows реестр: " + datetime.fromtimestamp(windows_installation_date_registry).strftime('%Y-%m-%d %H:%M:%S') + "\n")
        # print(Fore.YELLOW + f"Дата установки Windows wmi: {windows_installation_date_wmi}\n")
        print(Fore.YELLOW + f"Дата установки Windows powershell: {get_windows_install_date()}\n")
        report_file.write("\n=== Версия Windows ===\n")
        report_file.write(f"{windows_version_info}\n")
        # report_file.write(f"Дата установки Windows: {windows_installation_date}\n")
        # report_file.write(f"Дата установки Windows реестр: {datetime.fromtimestamp(windows_installation_date_registry).strftime('%Y-%m-%d %H:%M:%S')}\n")
        # report_file.write(f"Дата установки Windows wmi: {windows_installation_date_wmi}\n")
        report_file.write(f"Дата установки Windows powershell: {get_windows_install_date()}\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    is_vm, vm_indicators = check_virtual_machine()

    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write("\n9. Проверка на виртуальную машину\n")
        if is_vm:
            report_file.write("Обнаружены следующие признаки виртуальной машины:\n")
            for indicator in sorted(set(vm_indicators)):
                report_file.write(f"  • {indicator}\n")
            
            vm_types = {
                "VMware": sum(1 for i in vm_indicators if "VMware" in i),
                "VirtualBox": sum(1 for i in vm_indicators if "VirtualBox" in i),
                "Hyper-V": sum(1 for i in vm_indicators if "Hyper-V" in i),
                "QEMU": sum(1 for i in vm_indicators if "QEMU" in i),
                "Xen": sum(1 for i in vm_indicators if "Xen" in i),
                "KVM": sum(1 for i in vm_indicators if "KVM" in i)
            }
            probable_vm = max(vm_types.items(), key=lambda x: x[1])
            if probable_vm[1] > 0:
                report_file.write(f"\nВероятная виртуальная машина: {probable_vm[0]} (найдено {probable_vm[1]} признаков)\n")
        else:
            report_file.write("Признаков виртуальной машины не обнаружено\n")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при записи результатов проверки на VM: {e}")

    find_and_remove_enrage_files()

   
    try:
        report_file = open(report_filepath, "a", encoding="utf-8")
        report_file.write(f"\nОтчет создан: {today}\n")
        report_file.write("Все необходимые проверки выполнены.")
        report_file.close()
    except Exception as e:
        print(f"Ошибка при создании итогового отчета: {e}")

    input("\nНажмите Enter для выхода...")


if __name__ == "__main__":
    main()
