import requests

base_url = None
slot = 1
path_list="/api/fake_list"

try:
    response = requests.get(
        f"{base_url}{path_list}",
        params={"slot": slot},
        timeout=5,
    )
    print(f"HTTP {response.status_code}")
    print(response.text)
except requests.RequestException as error:
    print(f"请求失败：{error}")