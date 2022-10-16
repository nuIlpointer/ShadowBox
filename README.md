# ShadowBox
2023年学園祭に向けたプロジェクト「ShadowBox」用リポジトリ
## About

2023年の学園祭に向けて開発中のサンドボックス型ゲーム「ShadowBox」のサーバーサイドの開発で利用する(現状は)プライベートリポジトリ。

## ToDo

とりあえず年内に動くサーバー実装を完成させることを目標とする。

・著者の担当区分は「サーバーサイド全般」「クライアント側のサーバー通信ラッパー」。~~多くね？~~

・今月中:開発環境の整備・UDP通信(Unity Transportパッケージを利用)のテストを完成させる。

・プロジェクトの学内向けプレゼンの終了後:サーバーサイド→ラッパーパッケージ の順で開発に取り掛かる。予定。

## All

開発で用いるUnity バージョン:2020.3(マイナーバージョンの差異って問題発生するか？するなら調整必要かも...)

## Server

「ゲームのデータ管理」を担い、内部通信を汎用的なものにすることでマルチ/シングルの双方に対応させる。(マイクラみたいな)

実装はC#、根本的な通信を担うのはUnity Transport。