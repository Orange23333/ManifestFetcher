# 如何设置服务器

首先下载`ServerManifestGenerater`，并确保服务器安装了`gpg`。

在`目标文件夹`下编辑一个`config.mf.yml`文件：
```yaml
gpg_cmd: "gpg"
key_id: "你的pgp的key_id"
# `digest_algorithm` allow md5, sha1, sha256 (sha2-256), sha512 (sha2-512) as parameter.
digest_algorithm: "sha2-256"
```

然后执行：
```bash
ServerManifestGenerater 目标文件夹
```

# ServerManifestGenerater 功能

## 支持

* 自动化生成目录下的所有文件的摘要，并签名。

## 不支持

* 暂时不支持递归文件目录。
