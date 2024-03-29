# 스마트팩토리 MES 구축(Smart factory MES program)

![image](https://user-images.githubusercontent.com/57824945/236276192-f89f9061-f41e-4dbf-b845-72a2d8b9a64c.png)

<br/>

## 프로젝트 소개

이 프로젝트는 시설 내 설비와 기계에 설치된 센서들을 통해 데이터를 실시간으로 수집하고 분석하여 공장 내 모든 상황을 종합적으로 관리하는 스마트팩토리 프로젝트입니다. 자동차 부품을 만들기 위한 모든 공정들을 작업자가 없이 자동화하여 제어자 한 명만으로 많은 라인의 대규모 생산을 가능하게 하는 것이 목표입니다.

<br/>

### 프로젝트 구조

제품은 두 가지의 제품이 연동하여 생산됩니다. 이 두 제품은 바코드로 구분되며 바코드 출력 기계와 VISION 프로그램이 TCP/IP 통신을 통해 연결되어 있습니다. 
각 공정마다 COGNEX 사의 바코드 리더기로 제품을 확인하고 여러 공정들(밸런스검사, 성능검사, 배출검사, 압입검사, 저항검사 등)의 컬럼을 미리 데이터베이스에 생성합니다. 
Mitsubishi PLC와 VISION 프로그램이 MELSEC 통신을 통해 통신하며, PLC에서 각 공정의 값을 VISION 프로그램으로 전송하여 해당 값을 확인한 후 적절한 컬럼의 데이터베이스에 저장합니다. 별도로 자동차 부품의 밸런스 검사를 위해 밸런스 검사 VISION 장비를 사용하여 밸런싱 테스트를 진행합니다.

<br/>

## 사용 기술

- C#
- Mitsubishi PLC 및 MELSEC 통신
- 제브라사의 라벨 프린트 및 TCP/IP 통신
- 생산현황 모니터와 두 대의 DATAPC 및 TCP/IP 통신
- MySQL 데이터베이스

<br/>

## 프로젝트 리뷰

이 프로젝트를 통해 혼자서 자동화 라인의 통신 및 MES를 구축해보았습니다. 처음에는 다양한 디바이스 및 장비들과의 통신에서 어려움을 겪었지만 이를 해결해 나가는 과정에서 많은 배움을 얻을 수 있었습니다. 
또한 직접 데이터베이스를 구축하고 데이터를 쌓아 모니터에 표시함으로써 PC와 센서 간의 데이터베이스에 대한 이해도를 향상시켰습니다.


