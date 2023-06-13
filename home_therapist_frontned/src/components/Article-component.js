import React, { useState } from "react";
import styled from "styled-components";
import { Button } from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";
import "../css/style.css";

const ArticleForm = () => {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [author, setAuthor] = useState("");
  const [articles, setArticles] = useState([]);

  const handleTitleChange = (event) => {
    setTitle(event.target.value);
  };

  const handleContentChange = (event) => {
    setContent(event.target.value);
  };

  const handleAuthorChange = (event) => {
    setAuthor(event.target.value);
  };

  const handleSubmit = (event) => {
    event.preventDefault();

    if (author.length < 2 || author.length > 15) {
      console.log("内容必须3~15");
      return;
    }

    if (title.length < 5 || title.length > 20) {
      console.log("内容必须6~20");
      return;
    }

    if (content.length < 3 || content.length > 250) {
      console.log("内容必须4~249");
      return;
    }

    // 創建文章
    const newArticle = {
      id: Date.now(),
      title: title.length > 15 ? title.substring(0, 15) + "..." : title,
      content: content.length > 15 ? content.substring(0, 15) + "..." : content,
      author,
      publishedAt: new Date().toLocaleString()
    };

    setArticles([...articles, newArticle]);

    setTitle("");
    setContent("");
    setAuthor("");
  };

  const handleEdit = (id) => {
    const Edit = articles.find((article) => article.id === id);
    setTitle(Edit.title);
    setContent(Edit.content);
    setAuthor(Edit.author);
  };

  const handleDelete = (id) => {
    const updatedArticles = articles.filter((article) => article.id !== id);
    setArticles(updatedArticles);
  };

  return (
    <div className="container vh-100 pt-5" style={{marginBottom:"200px"}} >
      <FormFlex  onSubmit={handleSubmit}>
        <input 
          type="text"
          placeholder="標題"
          value={title}
          onChange={handleTitleChange}
          required
        />
        <input
          type="text"
          placeholder="作者姓名"
          value={author}
          onChange={handleAuthorChange}
          required
        />
        <textarea
          placeholder="請輸入内容，内容必须至少包含30個字符且不能超過250個字"
          value={content}
          onChange={handleContentChange}
          required
        />
        <FromButton
          style={{ marginLeft: "-40%", marginTop: "20px" }}
          type="submit"
        >
          提交
        </FromButton>
      </FormFlex>

      <h5>已發布的文章</h5>
      <div style={{ marginTop: "20px",border:"1px solid #ccc"}}>
        <div
          className="d-flex justify-content-between  p-2 "
          style={{
            backgroundColor: "#16f1d7",
            padding: "20px"
          }}
        >
          <div>作者</div>
          <div>標題</div>
          <div>內容</div>
          <div>上架時間</div>
          <div></div>
        </div>
        {articles.map((article) => (
          <FlexStyle  key={article.id} >
            <FromP>{article.author}</FromP>
            <FromP>{article.title}</FromP>
            <FromP>{article.content}</FromP>
            <FromP>{article.publishedAt}</FromP>
            <EditOutlined
              className="btn small-edit rounded-0"
              style={{
                display: "block",
                fontSize: "20px"
              }}
              onClick={() => handleEdit(article.id)}
            />
            <DeleteOutlined
              className="btn small-del rounded-0"
              style={{
                display: "block",
                fontSize: "20px",
                
              }}
              onClick={() => handleDelete(article.id)}
            />
          </FlexStyle>
        ))}
      </div>
    </div>
  );
};

export default ArticleForm;
const FlexStyle = styled.div`
  display: flex;
  border: 1px solid #ccc;

`;
const FromP = styled.div`
  width: 23%;
  //   background-color: blue;
  padding: 10px 2px;
`;

const FormFlex = styled.form`
padding-top:80px;
// background-color:red;
display:flex;
align-items: center;
flex-direction: column;
height:80vh;

input{
border:2px solid #06c1ab;
box-shadow: 0px 5px 15px rgba(0,0,0,0.5);
border-radius:2px;
width:50%;
padding:5px;
margin:10px 0;

}
textarea{
    border:2px solid #06c1ab;
    box-shadow: 0px 5px 15px rgba(0,0,0,0.5);
border-radius:2px;
margin-top:10px;
    width:50%;
padding:5px;
   height: 200px;
    
}


}


`;
const FromButton = styled.button`
  padding: 5px 30px;
  border-radius: 5px;
  border: 1px solid #06c1ab;
  text-shadow: rgba(77, 77, 0, 0.8);
  color: #06c1ab;

  &:hover {
    /* 悬停时的样式 */
    background-color: #0e8a80;
    box-shadow: rgba(0, 0, 0, 0.6);
    color: #fff;
  }
`;
