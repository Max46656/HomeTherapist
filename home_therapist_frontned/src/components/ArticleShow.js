import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";
import { LayoutMarTop } from "./style";

const ArticleShow = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);

  useEffect(() => {
    axios
      .get(`https://localhost:5000/api/Articles/${id}`)
      .then((res) => {
        console.log(res);
        const data = res.data.data;
        // 解析 body 中的 JSON，並取出 content
        data.body = JSON.parse(data.body);
        setArticle(data);
      })
      .catch((err) => console.log(err));
  }, [id]);

  if (!article) {
    return null;
  }

  return (
    <div>
      <br />
      <LayoutMarTop />
      <div>
        <h2>{article.title}</h2>
        {article.subtitle && <h3>{article.subtitle}</h3>}
        <div dangerouslySetInnerHTML={{ __html: article.body.content }} />
      </div>
    </div>
  );
};

export default ArticleShow;
