import requests

url = None
slot = 1  # 存档槽1
path_list="/api/fake_list"
path_save="/fake_save"
path_add="/api/fake_add"
path_clear="/api/fake_clear"

items = [
    (1001, "布料碎片"),
    (1002, "疗愈草药"),
    (1003, "移速草药"),
    (1004, "攻击草药"),
]

test_values = [
    ("正常值5", 5, "success"),
    ("正常值1", 1, "success"),
    ("正常值10", 10, "success"),
    ("边界值11", 11, "fail"),
    ("边界值0", 0, "fail"),
    ("负数-1", -1, "fail"),
    ("超大值", 123456789, "fail"),
    ("字符串数字", "10", "fail"),
    ("字符串字母", "abc", "fail"),
    ("浮点数", 1.5, "fail"),
    ("空值None", None, "fail"),
    ("布尔值True", True, "fail"),
]


def get_resources():
    response = requests.get(
        f"{url}{path_list}",
        params={"slot": slot},
        timeout=5,
    )
    response.raise_for_status()
    return response.json()["resources"]


def get_count(resources, item_id):
    for resource in resources:
        if int(resource["id"]) == item_id:
            return int(resource["count"])
    return 0


try:
    print("可测试资源：")
    for item_id, item_name in items:
        print(f"{item_name}{item_id}")

    item_id = int(input("\n测试资源id："))

    # 循环 12 个测试用例
    for desc, count, expected in test_values:
        print("\n" + "="*50)
        print(f"用例：{desc} | id={item_id} | count={count} | 预期结果={expected}")

        # 存档检查
        save_response = requests.get(
            f"{url}{path_save}",
            params={"slot": slot},
            timeout=5,
        )
        if save_response.status_code != 200:
            print(f"没有存档：HTTP {save_response.status_code} {save_response.text}")
            break

        # 清空
        clear_response = requests.post(
            f"{url}{path_clear}",
            params={"slot": slot},
            timeout=5,
        )
        if clear_response.status_code != 200:
            print(f"清空失败：HTTP {clear_response.status_code} {clear_response.text}")
            break

        before = get_resources()

        # 发请求
        response = requests.post(
            f"{url}{path_add}",
            params={"slot": slot},
            json={"id": item_id, "name": "测试物品", "count": count},
            timeout=5,
        )

        after = get_resources()

        print(f"HTTP状态码：{response.status_code}")
        if response.status_code == 200:
            print("请求成功")
        else:
            print(f"请求失败")
        print(f"Response：{response.text}")
        print(f"服务器json保存的数据（前）：{before}")
        print(f"服务器json保存的数据（后）：{after}")

except requests.RequestException as error:
    print(f"请求失败：{error}")
