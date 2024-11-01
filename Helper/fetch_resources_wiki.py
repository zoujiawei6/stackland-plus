import requests
from bs4 import BeautifulSoup
import re
import pandas as pd

# 获取 Stacklands wiki 的 resource 类别页面
url = "https://stacklands.fandom.com/wiki/Category:Resources"
headers = {
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3'
}

# 发送 HTTP 请求，获取网页内容
response = requests.get(url, headers=headers)
soup = BeautifulSoup(response.content, 'html.parser')

# 解析页面中 class 为 'category-page__members' 下的所有 'li' 元素
# 将所有 'li.category-page__member' 的文本内容作为数组 resources
resources = []
category_members = soup.select(".category-page__members li.category-page__member")
for member in category_members:
    resources.append(member.get_text().strip())

# 定义函数用于请求每个 resource 的页面，并解析其中的数据
def parse_resource_page(resource_name):
    # 将 resource_name 替换 URL 中的 {resource}，构建请求
    resource_url = f"https://stacklands.fandom.com/wiki/{resource_name.replace(' ', '_')}"
    resource_response = requests.get(resource_url, headers=headers)
    resource_soup = BeautifulSoup(resource_response.content, 'html.parser')

    # 解析 class 为 'pi-item pi-data' 的元素，找到其中的 h3 和 div
    pi_items = resource_soup.select(".pi-item.pi-data")
    resource_info = {}
    
    # 循环每个 pi-item 元素，分别取出 h3 和 div 的内容
    for item in pi_items:
        # 获取 h3 元素作为表头
        header = item.find('h3')
        # 获取 div 元素作为内容
        value = item.find('div')
        
        if header and value:
            # 添加到 resource_info 字典中
            header_text = header.get_text(strip=True).replace('|', ';')
            value_text = value.get_text(strip=True).replace('|', ';')
            resource_info[header_text] = value_text
    
    return resource_info

# 初始化 DataFrame 用于存储所有食物信息
data = []

# 遍历 resources 数组，获取每个食物页面的数据，并将其添加到 DataFrame 中
for resource in resources:
    resource_data = parse_resource_page(resource)
    row = {}
    row["Resource"] = resource
    for key, value in resource_data.items():
        row[key] = value
    data.append(row)
    print(row)

columns = ["Resource"]

for item in data:
    for key, value in item.items():
        if key not in columns:
            columns.append(key)

# 创建 DataFrame 并添加数据
df = pd.DataFrame(data, columns=columns)

# 将 DataFrame 保存为 Markdown 文件并输出
with open("wiki_resources_en.md", "w", encoding="utf-8") as f:
    f.write(df.to_markdown(index=False))

# 打印英文版表格
print(df.to_markdown(index=False))
