import requests

base_url = None
slot = 1
path_remove = "/api/v1/fake_remove"
path_list = "/api/fake_list"


def get_resources():
    response = requests.get(
        f"{base_url}{path_list}",
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
    before = get_resources()
    print(f"存档槽 {slot} 当前资源：")
    for resource in before:
        print(resource)

    item_id = int(input("\n要减少的资源id（布料碎片1001 疗愈草药1002 移速草药1003 攻击草药1004）："))
    count = int(input("减少数量："))

    old_count = get_count(before, item_id)

    name="测试资源"
    for r in before:
        if r["id"] == item_id:
            name = r.get("name","测试资源")
            break

    print(f"\nid={item_id}，当前数量={old_count}，请求数量={count}")
    if input("输入Y确认发送：") != "Y":
        raise SystemExit("已取消")

    response = requests.post(
        f"{base_url}{path_remove}",
        params={"slot": slot},
        json={"id": item_id, "name": name, "count": count},
        timeout=5,
    )
    print(f"\n接口响应：HTTP {response.status_code} {response.text}")

    after = get_resources()
    new_count = get_count(after, item_id)
    print(
        f"资源id {item_id}：{old_count} → {new_count}，"
        f"实际变化 {new_count - old_count}"
    )


except requests.RequestException as error:
    print(f"请求失败：{error}")
